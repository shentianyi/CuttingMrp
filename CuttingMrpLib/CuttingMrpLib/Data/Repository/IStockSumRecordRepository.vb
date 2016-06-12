Public Interface IStockSumRecordRepository
    Function SearchSR(conditions As DashboardSearchModel) As IQueryable(Of StockSumRecord)
End Interface
