﻿Imports Repository
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
        For Each m As MP In repo.GetTable
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
            repo.MarkForDeletion(m)
        Next
        Dim requireRepo As Repository(Of Requirement) = New Repository(Of Requirement)(dc)
        requireRepo.GetTable.InsertAllOnSubmit(requires)
        Using trans As New TransactionScope
            Try
                requireRepo.SaveAll()
                repo.SaveAll()
                trans.Complete()
            Catch ex As Exception
                Throw ex
            Finally
                'release resource
                repo = Nothing
                tempBoms = Nothing
                requires = Nothing
                requireRepo = Nothing
            End Try
        End Using
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
                    .status = ProcessOrderStatus.Open, .sourceQuantity = sum, .actualQuantity = actualQty,
                    .completeRate = completeRate, .batchQuantity = currPart.moq, .OrderDerivations = en}
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
                    stockRecords(requires.partNr) = Nothing
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
                    Dim delta As Integer = DayOfWeek.Monday - coll.requiredDate.DayOfWeek
                    key = coll.requiredDate.AddDays(delta).ToString("yyyy-MM-dd")
                Case "MONTH"
                    key = coll.requiredDate.ToString("yyyy-MM") & "-01"
                Case "YEAR"
                    key = coll.requiredDate.ToString("yyyy") & "-01-01"
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
