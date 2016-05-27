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

    Public Function Search(conditions As ProcessOrderSearchModel) As List(Of ProcessOrder) Implements IProcessOrderService.Search
        Dim reqRepo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Return reqRepo.Search(conditions).ToList
    End Function
End Class
