Imports Repository
Imports System.Globalization
Imports System.Data.Linq

Public Class Calculator
    Inherits ServiceBase
    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Sub ProcessMrp(settings As CalculateSetting)
        Dim mrprepo As Repository(Of MrpRound) = New Repository(Of MrpRound)(New DataContext(DBConn))
        Dim mrpRoundStr As String = Now.ToString("yyyyMMddhhmmss")
        mrprepo.GetTable.InsertOnSubmit(New MrpRound With {.launcher = My.Application.Info.AssemblyName, .mrpRound = mrpRoundStr, .runningStatus = CalculatorStatus.Running, .time = Now})
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

        ResetOrders({ProcessOrderEnum.Open}, settings.ReservedType, ProcessOrderEnum.SystemCancel)
        Dim toInsertOrders As List(Of ProcessOrder) = New List(Of ProcessOrder)
        Dim procOrderRepo As Repository(Of ProcessOrder) = New Repository(Of ProcessOrder)(New DataContext(DBConn))
        Dim orders As Hashtable = GetValidRequirement()
        Dim toInsert As List(Of ProcessOrder) = PrepareData(mrpRound, orders, settings)
        procOrderRepo.GetTable.InsertAllOnSubmit(toInsert)
        procOrderRepo.SaveAll()
    End Sub

    Private Sub ResetOrders(targetStatus() As ProcessOrderEnum, reserveTypes As List(Of String), status As ProcessOrderEnum)
        If reserveTypes Is Nothing Then
            reserveTypes = New List(Of String)
        End If
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(My.Settings.db))
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
            Dim ordernr As String = NumericService.GenerateID(DBConn, "PROCESSORDER")
            For Each piece As DictionaryEntry In orderPieces
                Dim sum As Double = 0
                Dim toInsertRefer As List(Of OrderDerivation) = New List(Of OrderDerivation)
                For Each req As Requirement In piece.Value
                    sum = sum + req.quantity
                    toInsertRefer.Add(New OrderDerivation With {.orderId = ordernr, .mrpRound = mrpround, .deriveQty = req.quantity, .requirementId = req.id})
                Next
                Dim dateresult As DateTime
                DateTime.TryParse(piece.Key, dateresult)
                Dim en As EntitySet(Of OrderDerivation) = New EntitySet(Of OrderDerivation)
                en.AddRange(toInsertRefer)
                Dim partRepo As Repository(Of Part) = New Repository(Of Part)(New DataContext(DBConn))
                Dim currPart As Part = partRepo.First(Function(c) c.partNr = dic.Key)
                Dim actualQty As Double
                If sum > 0 Then
                    If sum < currPart.spq Then
                        actualQty = currPart.spq
                    Else
                        If sum Mod currPart.spq <> 0 Then
                            actualQty = sum + sum - (sum Mod currPart.spq)
                        End If
                    End If
                End If

                Dim completeRate As Double = 0
                If sum <> 0 And actualQty <> 0 Then
                    completeRate = actualQty / sum
                End If

                Dim toinsert As ProcessOrder = New ProcessOrder With {.orderNr = ordernr,
                    .partNr = dic.Key, .derivedFrom = "MRP", .proceeDate = dateresult,
                    .OrderDerivations = en, .requirementId = " ", .sourceDoc = " ",
                    .status = ProcessOrderEnum.Open, .sourceQuantity = sum, .actualQuantity = actualQty,
                    .completeRate = completeRate, .batchQuantity = currPart.moq}
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
        Dim toUse As IQueryable(Of Requirement) = requireRepo.Search(searchConditions)
        Dim parts As IQueryable(Of String) = (From t In toUse Select t.partNr)
        Dim stockrepo As Repository(Of SumOfStock) = New Repository(Of SumOfStock)(New DataContext(DBConn))
        Dim stocks As IEnumerable(Of SumOfStock) = stockrepo.FindAll(Function(c) parts.Contains(c.partNr))
        Dim orders As Hashtable = New Hashtable
        For Each requires In toUse
            For Each inv As SumOfStock In stocks
                If inv.SumOfStock >= requires.quantity Then
                    inv.SumOfStock = inv.SumOfStock - requires.quantity
                    requires = Nothing
                    Exit For
                Else
                    requires.quantity = requires.quantity - inv.SumOfStock
                    inv = Nothing
                End If
            Next
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
    Public Function GroupByDate(dateType As String, collections As List(Of Requirement))
        If collections Is Nothing Or dateType Is Nothing Then
            Throw New ArgumentNullException
        End If
        Dim result As Hashtable = New Hashtable

        For Each coll As Requirement In collections
            Dim key As String = ""
            Select Case dateType
                Case "DAY"
                    key = coll.requiredDate.ToString("yyyy-MM-dd")
                Case "WEEK"
                    'get the monday of each week
                    Dim delta As Integer = DayOfWeek.Monday - coll.requiredDate.DayOfWeek;
                    key = coll.requiredDate.AddDays(delta).ToString("yyyy-MM-dd")
                Case "MONTH"
                    key = coll.requiredDate.ToString("yyyy-MM") & "-01"
                Case "YEAR"
                    key = coll.requiredDate.ToString("yyyy") & "01-01"
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

End Class
