Public Interface IBomRepository
    Function Search(conditions As BomSearchModel) As IQueryable(Of BOM)
End Interface
