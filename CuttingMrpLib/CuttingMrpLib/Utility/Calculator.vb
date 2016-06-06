Imports Repository
Imports System.Globalization
Imports System.Data.Linq
Imports System.Transactions

Public Class Calculator
    Inherits ServiceBase
    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Sub ProcessMrp(settings As CalculateSetting)
        Dim mrprepo As Repository(Of MrpRound) = New Repository(Of MrpRound)(New DataContext(DBConn))
        Dim mrpRoundStr As String = Now.ToString("yyyyMMddhhmmss")
        mrprepo.GetTable.InsertOnSubmit(New MrpRound With {.launcher = My.Application.Info.AssemblyName, .mrpRound = mrpRoundStr, .runningStatus = CalculatorStatus.Running, .time = Now, .text = " "})
        mrprepo.SaveAll()
        Try
            GenerateProcessOrderByRequirement(mrpRoundStr, settings)
            Dim round As MrpRound = mrprepo.First(Function(c) c.mrpRound = mrpRoundStr)
            round.runningStatus = CalculatorStatus.Finish
            mrprepo.SaveAll()
        Catch ex As Exception
            Dim round As MrpRound = mrprepo.First(Function(c) c.mrpRound = mrpRoundStr)
            round.runningStatus = CalculatorStatus.Cancel
            round.text = "Fail to run MRP with following errors:" & ex.ToString
            mrprepo.SaveAll()
            Throw New Exception(round.text)
        End Try
    End Sub
    Public Sub GenerateProcessOrderByRequirement(mrpRound As String, settings As CalculateSetting)
        '0 Clear the ProcessOrders, set the status to SYSCAN yes
        '1 find all valid requirement
        '2 find all stock of the part
        '3 get net requirement
        '4 generate the order
        '5.find the related KANBAN card
        ConvertMpsToRequirement()
        ResetOrders({ProcessOrderStatus.Open}, settings.ReservedType, ProcessOrderStatus.SystemCancel)
        Dim toInsertOrders As List(Of ProcessOrder) = New List(Of ProcessOrder)
        Dim procOrderRepo As Repository(Of ProcessOrder) = New Repository(Of ProcessOrder)(New DataContext(DBConn))
        Dim orders As Hashtable = GetValidRequirement()
        Dim toInsert As List(Of ProcessOrder) = PrepareData(mrpRound, orders, settings)

        procOrderRepo.GetTable.InsertAllOnSubmit(toInsert.ToArray)
        procOrderRepo.SaveAll()
    End Sub

    Public Sub ConvertMpsToRequirement()
        Dim dc As DataContext = New DataContext(DBConn)
        Dim repo As Repository(Of MP) = New Repository(Of MP)(dc)
        Dim tempBoms As Hashtable = New Hashtable
        Dim requires As List(Of Requirement) = New List(Of Requirement)
        Dim mpses As List(Of MP) = repo.FindAll(Function(c) c.status = MPSStatus.plan And c.requiredDate >= Now.Date).ToList
        For Each m As MP In mpses
            If tempBoms.ContainsKey(m.partnr) = False Then
                Dim bomRepo As Repository(Of BOM) = New Repository(Of BOM)(dc)
                Dim counter As Integer = bomRepo.Count(Function(c) c.validFrom <= m.requiredDate And c.validTo >= m.requiredDate And c.partNr = m.partnr)
                If counter < 1 Then
                    Throw New Exception("没有找到相应的BOM")
                End If
                If counter > 1 Then
                    Throw New Exception("找到" & counter & "个生效的BOM")
                End If
                Dim bom As BOM = bomRepo.Single(Function(c) c.validFrom <= Now And c.validTo >= Now And c.partNr = m.partnr)
                tempBoms.Add(m.partnr, bom.BomItems.Where(Function(c) c.validFrom <= Now And c.validTo >= Now).ToList)
            End If
            For Each item As BomItem In tempBoms(m.partnr)
                requires.Add(New Requirement With {.derivedFrom = m.sourceDoc, .derivedType = m.source, .orderedDate = m.orderedDate, .requiredDate = m.requiredDate, .partNr = item.componentId, .status = RequirementStatus.Open, .quantity = item.quantity * m.quantity})
            Next
            ' repo.MarkForDeletion(m)
        Next

        Dim requireRepo As Repository(Of Requirement) = New Repository(Of Requirement)(New DataContext(DBConn))
        Dim todeactives As IEnumerable(Of Requirement) = requireRepo.FindAll(Function(c) c.status = RequirementStatus.Open)
        For Each todeactive As Requirement In todeactives
            todeactive.status = RequirementStatus.CancelSystem
        Next

        'Using trans As New TransactionScope
        Try
            ' requireRepo.SaveAll()
            requireRepo.GetTable.InsertAllOnSubmit(requires)
                requireRepo.SaveAll()
            ' trans.Complete()
        Catch ex As Exception
                Throw ex
            Finally
                'release resource
                repo = Nothing
                tempBoms = Nothing
                requires = Nothing
                requireRepo = Nothing
            End Try
        'End Using
    End Sub
    Public Sub ResetOrders(targetStatus() As ProcessOrderStatus, reserveTypes As List(Of String), status As ProcessOrderStatus)
        If reserveTypes Is Nothing Then
            reserveTypes = New List(Of String)
        End If
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Dim toCancel As IEnumerable(Of ProcessOrder) = repo.FindAll(Function(c) targetStatus.Contains(c.status) And reserveTypes.Contains(c.derivedFrom) = False)

        For Each toCancelOrder As ProcessOrder In toCancel
            toCancelOrder.status = status
        Next
        repo.SaveAll()
    End Sub


    Public Function PrepareData(mrpround As String, orders As Hashtable, settings As CalculateSetting) As List(Of ProcessOrder)
        Dim result As List(Of ProcessOrder) = New List(Of ProcessOrder)
        For Each dic As DictionaryEntry In orders
            Dim orderPieces As Hashtable = GroupByDate(settings.MergeMethod, dic.Value)
            Dim ordernr As String
            For Each piece As DictionaryEntry In orderPieces
                ordernr = NumericService.GenerateID(DBConn, "PROCESSORDER")
                Dim sum As Double = 0
                Dim toInsertRefer As List(Of OrderDerivation) = New List(Of OrderDerivation)
                For Each req As Requirement In piece.Value
                    sum = sum + req.quantity
                    toInsertRefer.Add(New OrderDerivation With {.mrpRound = mrpround, .deriveQty = req.quantity, .requirementId = req.id})
                Next
                Dim dateresult As DateTime
                DateTime.TryParse(piece.Key, dateresult)
                Dim en As EntitySet(Of OrderDerivation) = New EntitySet(Of OrderDerivation)
                en.AddRange(toInsertRefer)
                Dim partRepo As Repository(Of Part) = New Repository(Of Part)(New DataContext(DBConn))
                Dim spq As Double = 0
                Dim moq As Double = 0
                If settings.OrderType = OrderType.Fix Then
                    Dim kanbanrepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))
                    Dim kanban As BatchOrderTemplate = kanbanrepo.First(Function(c) c.partNr = CType(dic.Key, String))

                    spq = kanban.bundle
                        moq = kanban.batchQuantity

                ElseIf settings.OrderType = OrderType.Fix Then
                    Dim currPart As Part = partRepo.First(Function(c) c.partNr = CType(dic.Key, String))
                    spq = currPart.spq
                    moq = currPart.moq
                End If

                Dim actualQty As Double
                If sum > 0 Then
                    If sum < spq Then
                        actualQty = spq
                    Else
                        If (sum Mod spq) <> 0 Then
                            actualQty = sum + (spq - (sum Mod spq))
                        Else
                            actualQty = sum
                        End If
                    End If
                End If

                Dim completeRate As Double = 0
                If sum <> 0 And actualQty <> 0 Then
                    completeRate = actualQty / sum
                End If
                Dim sourceDoc As String = " "
                If settings.OrderType = "FIX" Then
                    Dim fixorderrepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))
                    Dim fixorders As List(Of BatchOrderTemplate) = (From kbors In fixorderrepo.GetTable Where kbors.partNr = CType(dic.Key, String) Select kbors).ToList
                    If fixorders.Count > 0 Then
                        sourceDoc = ""
                        For Each fo As BatchOrderTemplate In fixorders
                            If String.IsNullOrEmpty(sourceDoc) Then
                                sourceDoc = fo.orderNr
                            Else
                                sourceDoc = fo.orderNr & "/" & sourceDoc
                            End If
                        Next
                    End If

                End If
                Dim toinsert As ProcessOrder = New ProcessOrder With {.orderNr = ordernr,
                    .partNr = dic.Key, .derivedFrom = "MRP", .proceeDate = dateresult, .sourceDoc = sourceDoc,
                    .status = ProcessOrderStatus.Open, .sourceQuantity = sum, .actualQuantity = actualQty,
                    .completeRate = completeRate, .batchQuantity = moq, .OrderDerivations = en, .orderType = settings.OrderType}
                result.Add(toinsert)
            Next
        Next

        Return result
    End Function

    Public Function GetValidRequirement()
        Dim requireRepo As RequirementRepository = New RequirementRepository(New DataContext(DBConn))
        Dim searchConditions As RequirementSearchModel = New RequirementSearchModel
        searchConditions.DerivedType = DeriveType.MRP
        searchConditions.Status = RequirementStatus.Open
        Dim toUse As List(Of Requirement) = requireRepo.Search(searchConditions).ToList
        toUse = (From f In toUse Order By f.requiredDate, f.partNr Ascending).ToList
        Dim parts As List(Of String) = (From t In toUse Select t.partNr).ToList
        Dim stockrepo As Repository(Of SumOfStock) = New Repository(Of SumOfStock)(New DataContext(DBConn))
        Dim stocks As List(Of SumOfStock) = stockrepo.FindAll(Function(c) parts.Contains(c.partNr)).ToList
        Dim orders As Hashtable = New Hashtable
        Dim stockRecords As Hashtable = New Hashtable
        For Each sto As SumOfStock In stocks
            If stockRecords.ContainsKey(sto.partNr) Then
                stockRecords(sto.partNr) = stockRecords(sto.partNr) + sto.SumOfStock
            Else
                stockRecords(sto.partNr) = sto.SumOfStock
            End If
        Next
        For Each requires In toUse
            If stockRecords.ContainsKey(requires.partNr) Then
                If stockRecords(requires.partNr) >= requires.quantity Then
                    stockRecords(requires.partNr) = stockRecords(requires.partNr) - requires.quantity
                    requires = Nothing
                Else
                    requires.quantity = requires.quantity - stockRecords(requires.partNr)
                    stockRecords.Remove(requires.partNr)
                End If
            End If

            If requires IsNot Nothing Then
                If orders.ContainsKey(requires.partNr) Then
                    orders(requires.partNr).add(requires)
                Else
                    Dim newValue As List(Of Requirement) = New List(Of Requirement)
                    newValue.Add(requires)
                    orders.Add(requires.partNr, newValue)
                End If
            End If
        Next
        Return orders
    End Function

    ''' <summary>
    ''' to group the requirement according to the MRP settings
    ''' </summary>
    ''' <param name="dateType">
    '''     "DAY" 
    '''     "WEEK"
    '''     "MONTH"
    '''     "YEAR"
    ''' </param>
    ''' <param name="collections"></param>
    ''' <returns></returns>
    Public Function GroupByDate(dateType As MergeMethod, collections As List(Of Requirement))
        If collections Is Nothing Or dateType Is Nothing Then
            Throw New ArgumentNullException
        End If
        Dim result As Hashtable = New Hashtable
        collections = (From coll In collections Select coll Order By coll.requiredDate Ascending).ToList
        For Each coll As Requirement In collections
            Dim key As String = ""
            Select Case dateType.MergeType
                Case "DAY"
                    If dateType.Count < 1 Then
                        Throw New Exception("Unsupported count" & dateType.Count & " of DAY method")
                    Else
                        '有需求日期小于基准日的订单
                        If coll.requiredDate < dateType.FirstDay Then

                        Else
                            If coll.requiredDate < dateType.FirstDay.AddDays(dateType.Count) Then
                                key = dateType.FirstDay.ToString("yyyy-MM-dd")
                            Else
                                key = FindBasicDate(dateType.FirstDay, coll.requiredDate, dateType.Count).ToString("yyyy-MM-dd")
                            End If
                        End If

                    End If
                Case "WEEK"
                    'get the monday of each week
                    Dim delta As Integer = DayOfWeek.Monday - coll.requiredDate.DayOfWeek
                    key = coll.requiredDate.AddDays(delta).ToString("yyyy-MM-dd")
                    Throw New NotImplementedException
                Case "MONTH"
                    key = coll.requiredDate.ToString("yyyy-MM") & "-01"
                    Throw New NotImplementedException
                Case "YEAR"
                    key = coll.requiredDate.ToString("yyyy") & "-01-01"
                    Throw New NotImplementedException
                Case Else
                    Throw New Exception("Unsupported Merge Method")
            End Select
            If result.ContainsKey(key) Then
                result(key).add(coll)
            Else
                Dim content As List(Of Requirement) = New List(Of Requirement)
                content.Add(coll)
                result.Add(key, content)
            End If
        Next
        Return result
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="firstBasicDate">基准日</param>
    ''' <param name="currentDate">当前时间（需要计算归属合并日的时间）</param>
    ''' <param name="count">时间跨度</param>
    ''' <returns></returns>
    Public Function FindBasicDate(firstBasicDate As Date, currentDate As Date, count As Integer) As Date
        Dim returnedDate As Date
        Dim daydiff As Integer
        If currentDate < firstBasicDate Then
            If Math.Floor((firstBasicDate - currentDate).TotalDays) < count Then
                returnedDate = firstBasicDate.AddDays(-count)
            Else
                Dim element As Double = (firstBasicDate - currentDate).TotalDays
                daydiff = -(Math.Floor((firstBasicDate - currentDate).TotalDays) Mod count)
                returnedDate = currentDate.AddDays(daydiff)
            End If
        Else
            If Math.Ceiling((currentDate - firstBasicDate).TotalDays) < count Then
                returnedDate = firstBasicDate
            Else
                daydiff = -(Math.Ceiling((currentDate - firstBasicDate).TotalDays) Mod count)
                returnedDate = currentDate.AddDays(daydiff)
            End If

        End If
        Return returnedDate
    End Function

    Public Sub MakeBackflush()
        'get backflush
        'make backflush
        'book stock
        '
        '1 - minus the forecast
        '2 - minus the material stock
        '3 - submit change
        Dim tempBoms As Hashtable = New Hashtable
        Dim dc As DataContext = New DataContext(DBConn)
        Dim mprrepo As Repository(Of MP) = New Repository(Of MP)(dc)
        Dim mpses As List(Of MP) = mprrepo.FindAll(Function(c) c.status = MPSStatus.Backflush).ToList
        Dim backFlushRepo As Repository(Of BackflushRecord) = New Repository(Of BackflushRecord)(dc)
        Dim backflushSucc As List(Of BackflushRecord) = New List(Of BackflushRecord)
        Dim backflushFail As List(Of BackflushRecord) = New List(Of BackflushRecord)
        Dim stockToBook As Hashtable = New Hashtable
        Dim bomRepo As Repository(Of BOM) = New Repository(Of BOM)(dc)
        Dim stockRepo As Repository(Of Stock) = New Repository(Of Stock)(dc)
        If mpses IsNot Nothing Then
            Try
                For Each m As MP In mpses
                    If tempBoms.ContainsKey(m.partnr) = False Then

                        Dim counter As Integer = bomRepo.Count(Function(c) c.validFrom <= m.requiredDate And c.validTo >= m.requiredDate And c.partNr = m.partnr)
                        If counter < 1 Then
                            backflushFail.Add(New BackflushRecord With
                                              {.fifo = Now, .launchTime = Now,
                                              .message = "没有找到BOM",
                                              .partnr = m.partnr,
                                              .quantity = m.quantity,
                                              .sourceDoc = m.sourceDoc,
                                              .status = BackflushStatus.Normal})
                        End If
                        If counter > 1 Then
                            backflushFail.Add(New BackflushRecord With
                                              {.fifo = Now, .launchTime = Now,
                                              .message = "找到" & counter & "个生效的BOM",
                                              .partnr = m.partnr, .quantity = m.quantity,
                                              .sourceDoc = m.sourceDoc,
                                              .status = BackflushStatus.Normal})
                        End If
                        Dim bom As BOM = bomRepo.Single(Function(c) c.validFrom <= Now And c.validTo >= Now And c.partNr = m.partnr)
                        tempBoms.Add(m.partnr, bom.BomItems.Where(Function(c) c.validFrom <= Now And c.validTo >= Now).ToList)
                    End If
                    For Each item As BomItem In tempBoms(m.partnr)
                        If stockToBook.ContainsKey(item.componentId) Then
                            stockToBook(item.componentId) = stockToBook(item.componentId) + item.quantity * m.quantity
                        Else
                            stockToBook(item.componentId) = item.quantity * m.quantity
                        End If
                    Next

                    'book stock
                    '1.find the related stocks
                    '2.book one by one until the stocktobook = 0
                    '3.reset the status of mp
                    '4.save all
                    Dim stockStrings As List(Of String) = New List(Of String)
                    For Each ke As Object In stockToBook
                        stockStrings.Add(CType(ke, String))
                    Next
                    Dim actualStock As List(Of Stock) = stockRepo.FindAll(Function(c) stockStrings.Contains(c.partNr)).ToList
                    actualStock = (From c In actualStock Order By c.fifo Ascending).ToList
                    Dim allocatedStock As Hashtable = New Hashtable
                    For Each st As Stock In actualStock
                        If allocatedStock.ContainsKey(st.partNr) Then
                            allocatedStock(st.partNr).add(st)
                        Else
                            Dim li As List(Of Stock) = New List(Of Stock)
                            li.Add(st)
                            allocatedStock.Add(st.partNr, li)
                        End If
                    Next
                    For Each dic As DictionaryEntry In stockToBook
                        If allocatedStock.ContainsKey(CType(dic.Key, String)) Then
                            For Each stockto As Stock In allocatedStock(CType(dic.Key, String))
                                If CType(dic.Value, Double) = stockto.quantity Then
                                    stockRepo.MarkForDeletion(stockto)
                                    Exit For
                                ElseIf CType(dic.Value, Double) < stockto.quantity Then
                                    stockto.quantity = stockto.quantity - CType(dic.Value, Double)
                                    Exit For
                                Else
                                    stockRepo.MarkForDeletion(stockto)
                                    dic.Value = CType(dic.Value, Double) - stockto.quantity
                                End If
                            Next
                        Else
                            '副库存

                        End If
                    Next

                Next
                dc.SaveAll()
            Catch ex As Exception

            End Try

        End If

    End Sub

End Class
