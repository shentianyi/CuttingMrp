Imports CuttingMrpLib

Public Class SumOfStockService
    Inherits ServiceBase
    Implements ISumOfStockService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function SearchSumOfStock(searchModel As SumOfStockSearchModel) As IQueryable(Of SumOfStock) Implements ISumOfStockService.SearchSumOfStock

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As IStockRepository = New StockRepository(context)

        Return rep.SearchSumOfStock(searchModel)
    End Function

End Class
