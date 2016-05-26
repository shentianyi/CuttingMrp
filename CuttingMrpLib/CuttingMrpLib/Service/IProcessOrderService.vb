Public Interface IProcessOrderService
    Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double)
    Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean)
    Function GenerateProcessOrderByRequirement(requirements As List(Of Requirement), reserveType As List(Of String)) As Boolean
End Interface
