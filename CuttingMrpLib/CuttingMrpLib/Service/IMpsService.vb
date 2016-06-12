Public Interface IMpsService
    Function Search(searchModel As MpsSeachModel) As IQueryable(Of MP)
End Interface
