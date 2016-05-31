﻿Public Interface IProcessOrderService

    Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder)
    Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double)
    Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean)
    Sub FinishOrdersByIds(ids As List(Of String), fifo As DateTime, container As String, wh As String, position As String, source As String, sourceType As String)
    Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate)

    Function FindById(id As String) As ProcessOrder
    Function DeleteById(id As String) As Boolean
    Function Update(processOrder As ProcessOrder) As Boolean
    Function GetProcessOrderInfo(conditions As ProcessOrderSearchModel) As ProcessOrderInfoModel
    Function BatchFinishOrder(records As List(Of BatchFinishOrderRecord), ignoreErrors As Boolean) As Boolean
    Function ValidateFinishOrder(records As List(Of BatchFinishOrderRecord)) As Hashtable 
End Interface
