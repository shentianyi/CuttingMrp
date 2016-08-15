Public Interface IUnDoneStockService
    Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock)

    Function Create(records As UnDoneStock) As Boolean

    Function ValidateUnDoneStock(records As List(Of UnDoneStockRecord)) As Hashtable

    Function SetStateCancel(type As PartType)
End Interface
