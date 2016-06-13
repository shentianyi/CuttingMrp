Public Interface ISumOfStockService
    Function SearchSumOfStock(searchModel As SumOfStockSearchModel) As IQueryable(Of SumOfStock)
End Interface
