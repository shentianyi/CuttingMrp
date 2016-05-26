Public Interface IStockService
    Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock)

End Interface
