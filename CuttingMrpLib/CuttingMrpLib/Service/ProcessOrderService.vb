Imports System.Transactions
Imports CuttingMrpLib
Imports Repository


Public Class ProcessOrderService
    Inherits ServiceBase
    Implements IProcessOrderService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub



    Public Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate) Implements IProcessOrderService.FindOrderTemplateByPartNr
        Dim fixOrderRepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))
        Return fixOrderRepo.FindAll(Function(c) c.partNr = partNr)
    End Function



    Public Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean) Implements IProcessOrderService.CancelOrdersByIds
        If ids Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
            Dim toCancel As IEnumerable(Of ProcessOrder) = repo.FindAll(Function(c) c.status = ProcessOrderStatus.Open And ids.Contains(c.orderNr))

            For Each toCancelOrder As ProcessOrder In toCancel
                If isSystem = True Then
                    toCancelOrder.status = ProcessOrderStatus.SystemCancel
                Else
                    toCancelOrder.status = ProcessOrderStatus.ManualCancel
                End If
            Next
            repo.SaveAll()
        End If
    End Sub

    Public Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double) Implements IProcessOrderService.UpdateOrderQuantity
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Dim toupdate As ProcessOrder = repo.First(Function(c) c.orderNr = toUpdateId)
        If toupdate Is Nothing Then
            Throw New Exception("Cannot find Order")
        Else
            toupdate.actualQuantity = quantity
        End If
        repo.SaveAll()
    End Sub

    Public Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder) Implements IProcessOrderService.Search
        Dim reqRepo As IProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Return reqRepo.Search(conditions)
    End Function

    Public Function DeleteById(id As String) As Boolean Implements IProcessOrderService.DeleteById
        Dim result As Boolean = False
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Dim order As ProcessOrder = repo.First(Function(c) c.orderNr = id)
        If order Is Nothing Then
            Throw New Exception("Cannot find Order")
        Else
            If order.canDelete Then
                repo.MarkForDeletion(order)
                repo.SaveAll()
                result = True
            Else
                Throw New Exception("Order cannot deleted!")
            End If
        End If
        Return result
    End Function

    Public Function FindById(id As String) As ProcessOrder Implements IProcessOrderService.FindById
        Dim processOrder As ProcessOrder = Nothing

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As ProcessOrderRepository = New ProcessOrderRepository(context)
        Return rep.First(Function(o) o.orderNr.Equals(id))
    End Function

    Public Function Update(processOrder As ProcessOrder) As Boolean Implements IProcessOrderService.Update
        Dim result As Boolean = False

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As ProcessOrderRepository = New ProcessOrderRepository(context)
        Dim uorder As ProcessOrder = rep.First(Function(s) s.orderNr.Equals(processOrder.orderNr))
        If (uorder IsNot Nothing) Then
            'uorder.proceeDate = processOrder.proceeDate

            uorder.actualQuantity = processOrder.actualQuantity
            uorder.status = processOrder.status

            rep.SaveAll()
            result = True
        End If
        Return result
    End Function

    Public Sub FinishOrdersByIds(ids As List(Of String), fifo As DateTime, container As String, wh As String, position As String, source As String, sourceType As String) Implements IProcessOrderService.FinishOrdersByIds
        If ids Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim context = New DataContext(DBConn)
            Dim repo As ProcessOrderRepository = New ProcessOrderRepository(context)

            Dim stockRep As StockRepository = New StockRepository(context)

            Dim toFinish As IEnumerable(Of ProcessOrder) = repo.FindAll(Function(c) c.status = ProcessOrderStatus.Open And ids.Contains(c.orderNr))
            Dim stocks As List(Of Stock) = New List(Of Stock)
            For Each toFinishOrder As ProcessOrder In toFinish
                stocks.Add(New Stock With {.partNr = toFinishOrder.partNr,
                           .fifo = fifo,
                           .quantity = toFinishOrder.actualQuantity,
                           .container = container,
                           .wh = wh,
                           .position = position,
                           .source = toFinishOrder.orderNr,
                           .sourceType = "ProcessOrder"})
                toFinishOrder.status = ProcessOrderStatus.Finish
            Next
            If stocks.Count > 0 Then
                stockRep.Inserts(stocks)
                stockRep.SaveAll()
            End If
        End If
    End Sub

    Public Function GetProcessOrderInfo(conditions As ProcessOrderSearchModel) As ProcessOrderInfoModel Implements IProcessOrderService.GetProcessOrderInfo

        Dim info As ProcessOrderInfoModel = New ProcessOrderInfoModel
        Dim context = New DataContext(DBConn)
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(context)
        Dim q As IQueryable(Of ProcessOrder) = repo.Search(conditions)


        info.latestOrder = q.OrderByDescending(Function(c) c.proceeDate).FirstOrDefault()
        info.oldestOrder = q.OrderBy(Function(c) c.proceeDate).FirstOrDefault()
        info.processOrderCount = q.Count
        info.requirementCount = context.Context.GetTable(Of OrderDerivation).Where(Function(o) q.Select(Function(c) c.orderNr).Contains(o.orderId)).Count
        'q.Select(Function(c) c.OrderDerivations).Distinct.Count


        Return info

    End Function



    Public Function BatchFinishOrder(records As List(Of BatchFinishOrderRecord), ignoreError As Boolean) As Boolean Implements IProcessOrderService.BatchFinishOrder
        Dim validated As Hashtable = ValidateFinishOrder(records)
        If validated.ContainsKey("WARN") Then
            If ignoreError = False Then
                Throw New Exception("文件验证失败")
            End If
        End If
        Dim conditions As ProcessOrderSearchModel = New ProcessOrderSearchModel With {.Status = ProcessOrderStatus.Open, .PageSize = Integer.MaxValue}
        Dim sortedOrders As Hashtable = New Hashtable
        Dim toUpdate As List(Of ProcessOrder) = New List(Of ProcessOrder)
        Dim toFinish As List(Of ProcessOrder) = New List(Of ProcessOrder)
        Dim existingOrders As List(Of ProcessOrder) = Me.Search(conditions).ToList
        existingOrders = (From order In existingOrders Select order Order By order.proceeDate Ascending).ToList
        'group by order nr
        For Each ord As ProcessOrder In existingOrders
            If sortedOrders.ContainsKey(ord.partNr) Then
                sortedOrders(ord.partNr).add(ord)
            Else
                Dim li As New List(Of ProcessOrder)
                li.Add(ord)
                sortedOrders(ord.partNr) = li
            End If
        Next


        For Each rec As BatchFinishOrderRecord In validated("SUCCESS")

        Next

        Using scope As New TransactionScope
            Try

            Catch ex As Exception
                Throw New Exception("写入数据库时出现错误", ex)
            End Try
        End Using
    End Function

    Private Function PartExists(partNr As String) As Boolean
        Dim partRepo As Repository(Of Part) = New Repository(Of Part)(New DataContext(DBConn))
        Dim counter As Integer = partRepo.Count(Function(c) c.partNr = partNr)
        If counter > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ValidateFinishOrder(records As List(Of BatchFinishOrderRecord)) As Hashtable Implements IProcessOrderService.ValidateFinishOrder
        If records Is Nothing Then
            Throw New ArgumentNullException
        End If
        Dim warning As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)
        Dim succ As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)
        For Each rec As BatchFinishOrderRecord In records
            If PartExists(rec.PartNr) = False Then
                rec.Warnings.Add("Part Nr " & rec.PartNr & " does not exist in the system")
                warning.Add(rec)
            Else

            End If
        Next
        Dim result As Hashtable = New Hashtable
        result.Add("WARN", warning)
        result.Add("SUCCESS", succ)
        Return result
    End Function
End Class
