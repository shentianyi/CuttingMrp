Public Interface IBomItemService
    Function Search(conditions As BomItemSearchModel) As IQueryable(Of BomItem)

    Function Create(bomItem As BomItem) As Boolean
    Function Update(bomItem As BomItem) As Boolean
    Function Delete(bomItem As BomItem) As Boolean
End Interface
