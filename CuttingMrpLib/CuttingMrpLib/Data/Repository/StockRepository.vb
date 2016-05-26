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
            Dim stocks As IQueryable(Of Stock) = _context.Stock

            If Not String.IsNullOrWhiteSpace(searchModel.PartNr) Then
                stocks = stocks.Where(Function(s) s.partNr.Contains(searchModel.PartNr))
            End If

            If searchModel.FIFOFrom.HasValue Then

                stocks = stocks.Where(Function(s) s.fifo >= searchModel.FIFOFrom.Value)

            End If

            If searchModel.FIFOTo.HasValue Then

                stocks = stocks.Where(Function(s) s.fifo <= searchModel.FIFOTo.Value)

            End If



            Return stocks
        End If
        Return Nothing
    End Function
End Class
