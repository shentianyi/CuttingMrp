Public Interface IProcessOrderRepository
    Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder)


End Interface
