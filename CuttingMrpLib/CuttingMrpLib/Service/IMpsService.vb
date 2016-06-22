Public Interface IMpsService
    Function Search(searchModel As MpsSeachModel) As IQueryable(Of MP)
    Function FindById(id As String) As MP
    Function DeleteById(id As String) As Boolean
    Function Update(mps As MP) As Boolean
    Function Create(mps As MP) As Boolean
End Interface
