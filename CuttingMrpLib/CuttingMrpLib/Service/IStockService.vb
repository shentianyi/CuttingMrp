Public Interface IStockService
    Function Search(searchModel As StockSearchModel) As IQueryable(Of Stock)
    Function FindById(id As Integer) As Stock
    Function DeleteById(id As Integer) As Boolean
    Function Update(stock As Stock) As Boolean
End Interface
