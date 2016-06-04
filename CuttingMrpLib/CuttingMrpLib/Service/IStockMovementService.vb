Public Interface IStockMovementService
    Function Search(searchModel As StockMovementSearchModel) As IQueryable(Of StockMovement)
End Interface
