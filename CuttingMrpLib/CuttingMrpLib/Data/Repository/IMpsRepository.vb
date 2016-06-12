Public Interface IMpsRepository
    Function Search(conditons As MpsSeachModel) As IQueryable(Of MP)
End Interface
