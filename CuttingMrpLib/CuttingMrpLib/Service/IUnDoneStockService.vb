Public Interface IUnDoneStockService
    Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock)

    Function Create(undonestock As UnDoneStock) As Boolean
End Interface
