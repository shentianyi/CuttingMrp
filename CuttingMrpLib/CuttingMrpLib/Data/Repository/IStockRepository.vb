Public Interface IStockRepository
    Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock)

End Interface
