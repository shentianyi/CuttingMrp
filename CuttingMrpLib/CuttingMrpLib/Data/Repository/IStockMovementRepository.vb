Public Interface IStockMovementRepository
    Function Search(conditions As StockMovementSearchModel) As IQueryable(Of StockMovement)
End Interface
