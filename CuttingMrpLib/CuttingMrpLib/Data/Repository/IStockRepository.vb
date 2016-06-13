Public Interface IStockRepository
    Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock)

    Function SearchSumOfStock(searchModel As SumOfStockSearchModel) As IQueryable(Of SumOfStock)
End Interface
