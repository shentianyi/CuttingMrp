Public Interface IStockMovementService
    Function Search(searchModel As StockMovementSearchModel) As IQueryable(Of StockMovement)
    'Function GetStockMovementInfo(conditions As StockMovementSearchModel) As StockMovementInfoModel
End Interface
