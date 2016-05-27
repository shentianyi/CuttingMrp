Public Interface IProcessOrderService

    Function Search(conditions As ProcessOrderSearchModel) As List(Of ProcessOrder)
    Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double)
    Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean)
    Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate)
End Interface
