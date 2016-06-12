Imports CuttingMrpLib
Imports Repository

Public Class StockSumRecordRepository
    Inherits Repository.Repository(Of DashboardForStockReportItem)
    Implements IStockSumRecordRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = Me._dataContextFactory.Context
    End Sub

    Public Function SearchSR(conditions As DashboardSearchModel) As IQueryable(Of StockSumRecord) Implements IStockSumRecordRepository.SearchSR
        If conditions IsNot Nothing Then
            Dim stockSumRecords As IQueryable(Of StockSumRecord) = _context.StockSumRecords

            If Not String.IsNullOrWhiteSpace(conditions.PartNr) Then
                stockSumRecords = stockSumRecords.Where(Function(c) c.partNr.Contains(conditions.PartNr))
            End If

            If conditions.DateFrom.HasValue Then
                stockSumRecords = stockSumRecords.Where(Function(c) c.date >= conditions.DateFrom.ToString())

            End If

            If conditions.DateTo.HasValue Then
                stockSumRecords = stockSumRecords.Where(Function(c) c.date <= conditions.DateTo.ToString())
            End If

            Return stockSumRecords
        End If
        Return Nothing
    End Function
End Class
