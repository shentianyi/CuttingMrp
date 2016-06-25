Public Interface IBomItemRepository
    Function Search(condition As BomItemSearchModel) As IQueryable(Of BomItem)
End Interface
