Imports CuttingMrpLib
Imports Repository

Public Class StockRepository
    Inherits Repository(Of Stock)
    Implements IStockRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = Me._dataContextFactory.Context
    End Sub

    Public Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock) Implements IStockRepository.Search
        If searchModel IsNot Nothing Then

            Dim stocks As IQueryable(Of Stock) = _context.Stocks
            If Not String.IsNullOrWhiteSpace(searchModel.PartNr) Then
                stocks = stocks.Where(Function(s) s.partNr.Contains(searchModel.PartNr))
            End If

            If searchModel.FIFOFrom.HasValue Then
                stocks = stocks.Where(Function(s) s.fifo >= searchModel.FIFOFrom.Value)
            End If

            If searchModel.FIFOTo.HasValue Then
                stocks = stocks.Where(Function(s) s.fifo <= searchModel.FIFOTo.Value)
            End If

            If searchModel.QuantityFrom.HasValue Then
                stocks = stocks.Where(Function(s) s.quantity >= searchModel.QuantityFrom.Value)
            End If

            If searchModel.QuantityTo.HasValue Then
                stocks = stocks.Where(Function(s) s.quantity <= searchModel.QuantityTo.Value)
            End If

            If Not String.IsNullOrWhiteSpace(searchModel.Wh) Then
                stocks = stocks.Where(Function(s) s.wh.Equals(searchModel.Wh))
            End If


            If Not String.IsNullOrWhiteSpace(searchModel.Position) Then
                stocks = stocks.Where(Function(s) s.position.Contains(searchModel.Position))
            End If

            Return stocks
        End If
        Return Nothing
    End Function

    Public Function SearchSumOfStock(searchModel As SumOfStockSearchModel) As IQueryable(Of SumOfStock) Implements IStockRepository.SearchSumOfStock
        If searchModel IsNot Nothing Then
            Dim sumOfStock As IQueryable(Of SumOfStock) = _context.SumOfStocks

            If Not String.IsNullOrWhiteSpace(searchModel.PartNr) Then
                sumOfStock = sumOfStock.Where(Function(s) s.partNr.Contains(searchModel.PartNr.Trim()))
            End If

            Return sumOfStock
        End If
        Return Nothing
    End Function
End Class
