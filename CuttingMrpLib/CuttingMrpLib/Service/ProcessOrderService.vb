﻿Imports System.Transactions
Imports Repository


Public Class ProcessOrderService
    Inherits ServiceBase
    Implements IProcessOrderService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    ''' <summary>
    ''' This operation needs large resource. Run under background
    ''' </summary>
    ''' <param name="requirements"></param>
    ''' <returns></returns>
    Public Function GenerateProcessOrderByRequirement(requirements As List(Of Requirement), reserveType As List(Of String), mrpRound As String) As Boolean Implements IProcessOrderService.GenerateProcessOrderByRequirement
        '0 Clear the ProcessOrders, set the status to SYSCAN yes
        '1 find all valid requirement
        '2 find all stock of the part
        '3 get net requirement
        '4 generate the order
        '5.find the related KANBAN card

        ResetOrders({ProcessOrderEnum.Open}, reserveType, ProcessOrderEnum.SystemCancel)
        Dim requireRepo As RequirementRepository = New RequirementRepository(New DataContext(DBConn))
        Dim searchConditions As RequirementSearchModel = New RequirementSearchModel
        searchConditions.DerivedType = DeriveType.MRP
        searchConditions.PageSize = Integer.MaxValue
        searchConditions.PageIndex = 0
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

        Dim toInsertOrders As List(Of ProcessOrder) = New List(Of ProcessOrder)
        Dim procOrderRepo As Repository(Of ProcessOrder) = New Repository(Of ProcessOrder)(New DataContext(DBConn))

        For Each dic As DictionaryEntry In orders
            Dim derivs As List(Of OrderDerivation) = New List(Of OrderDerivation)
            For Each req As Requirement In dic.Value
                derivs.Add(New OrderDerivation With {.mrpRound = mrpRound, .requirementId = req.id, .deriveQty = req.quantity})
            Next

            Dim toinsert As ProcessOrder = New ProcessOrder


        Next



    End Function

    Public Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate) Implements IProcessOrderService.FindOrderTemplateByPartNr
        Dim fixOrderRepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))
        Return fixOrderRepo.FindAll(Function(c) c.partNr = partNr)
    End Function

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

    Public Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean) Implements IProcessOrderService.CancelOrdersByIds
        If ids Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(My.Settings.db))
            Dim toCancel As IEnumerable(Of ProcessOrder) = repo.FindAll(Function(c) c.status = ProcessOrderEnum.Open And ids.Contains(c.orderNr) = False)

            For Each toCancelOrder As ProcessOrder In toCancel
                If isSystem = True Then
                    toCancelOrder.status = ProcessOrderEnum.SystemCancel
                Else
                    toCancelOrder.status = ProcessOrderEnum.ManualCancel
                End If
            Next
            repo.SaveAll()
        End If
    End Sub

    Public Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double) Implements IProcessOrderService.UpdateOrderQuantity
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(My.Settings.db))
        Dim toupdate As ProcessOrder = repo.First(Function(c) c.orderNr = toUpdateId)
        If toupdate Is Nothing Then
            Throw New Exception("找不到订单")
        Else
            toupdate.actualQuantity = quantity
        End If
        repo.SaveAll()
    End Sub
End Class
