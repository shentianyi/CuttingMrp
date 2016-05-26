Imports CuttingMrpLib

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
End Class
