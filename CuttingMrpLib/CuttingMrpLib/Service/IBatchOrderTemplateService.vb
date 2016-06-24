Public Interface IBatchOrderTemplateService
    Function Search(conditions As BatchOrderTemplateSearchModel) As IQueryable(Of BatchOrderTemplate)

    Function Create(batchOrderTempalte As BatchOrderTemplate) As Boolean
    Function Update(batchOrderTempalte As BatchOrderTemplate) As Boolean
    Function Delete(batchOrderTempalte As BatchOrderTemplate) As Boolean
End Interface
