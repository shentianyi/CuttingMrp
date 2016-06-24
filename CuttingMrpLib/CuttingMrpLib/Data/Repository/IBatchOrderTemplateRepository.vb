Public Interface IBatchOrderTemplateRepository
    Function Search(condition As BatchOrderTemplateSearchModel) As IQueryable(Of BatchOrderTemplate)
End Interface
