Public Interface IProcessOrderService

    Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder)
    Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double)
    Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean)
    Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate)

    Function FindById(id As String) As ProcessOrder
    Function DeleteById(id As String) As Boolean
    Function Update(processOrder As ProcessOrder) As Boolean
End Interface
