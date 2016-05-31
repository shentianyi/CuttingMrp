Public Interface IProcessOrderRepository
    Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder)

    Function SearchView(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrderView)
End Interface
