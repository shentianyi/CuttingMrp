Public Interface IProcessOrderService
    Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double)
    Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean)
    Function GenerateProcessOrderByRequirement(requirements As List(Of Requirement), reserveType As List(Of String), roundId As String) As Boolean

    Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate)
End Interface
