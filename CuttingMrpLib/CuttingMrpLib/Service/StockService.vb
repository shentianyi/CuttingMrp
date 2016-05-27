Imports CuttingMrpLib
Imports Repository

Public Class StockService
    Inherits ServiceBase
    Implements IStockService


    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock) Implements IStockService.Search
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As IStockRepository = New StockRepository(context)

        Return rep.Search(searchModel)
    End Function


    Public Function FindById(id As Integer) As Stock Implements IStockService.FindById
        Dim stock As Stock = Nothing
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Stock) = New Repository.Repository(Of Stock)(context)
            stock = rep.First(Function(s) s.id.Equals(id))
        Catch ex As Exception
            Dim s = ex.Message
        End Try
        Return stock
    End Function

    Public Function DeleteById(id As Integer) As Boolean Implements IStockService.DeleteById
        Dim result As Boolean = False
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Stock) = New Repository.Repository(Of Stock)(context)
            Dim stock As Stock = rep.First(Function(s) s.id.Equals(id))
            If (stock IsNot Nothing) Then
                rep.MarkForDeletion(stock)
                context.SaveAll()
                result = True
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function Update(stock As Stock) As Boolean Implements IStockService.Update
        Dim result As Boolean = False
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Stock) = New Repository.Repository(Of Stock)(context)
            Dim ustock As Stock = rep.First(Function(s) s.id.Equals(stock.id))
            If (ustock IsNot Nothing) Then
                ustock.fifo = stock.fifo
                ustock.quantity = stock.quantity
                ustock.container = stock.container
                ustock.wh = stock.wh
                ustock.position = stock.position
                ustock.source = stock.source
                ustock.sourceType = stock.sourceType


                context.SaveAll()
                result = True
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function
End Class
