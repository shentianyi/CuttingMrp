Public Interface IUnDoneStockRepository
    Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock)
End Interface
