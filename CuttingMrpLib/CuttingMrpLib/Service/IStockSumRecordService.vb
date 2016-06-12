Public Interface IStockSumRecordService
    Sub Generate(dateTime As DateTime)

    Function SearchStockReport(conditions As DashboardSearchModel) As IQueryable(Of StockSumRecord)
End Interface
