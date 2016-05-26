﻿Imports CuttingMrpLib
Imports Repository

Public Class StockService
    Implements IStockService
    Public Property dbString As String

    Public Sub New(dbString As String)
        Me.dbString = dbString
    End Sub
    Public Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock) Implements IStockService.Search
        Dim context As DataContext = New DataContext(Me.dbString)
        Dim rep As IStockRepository = New StockRepository(context)

        Return rep.Search(searchModel)
    End Function


    Public Function FindById(id As Integer) As Stock Implements IStockService.FindById
        Dim stock As Stock = Nothing
        Try
            Dim context As DataContext = New DataContext(Me.dbString)
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
            Dim context As DataContext = New DataContext(Me.dbString)
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

End Class
