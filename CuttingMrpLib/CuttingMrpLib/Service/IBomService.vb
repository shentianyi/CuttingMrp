Public Interface IBomService
    Function Search(conditions As BomSearchModel) As IQueryable(Of BOM)

    Function Create(bom As BOM) As Boolean
    Function Update(bom As BOM) As Boolean
    Function Delete(bom As BOM) As Boolean
End Interface
