Public Interface IPartService
    Function FindById(id As String) As Part
    Function FuzzyById(id As String) As List(Of Part)
    Function GetParentParts(partNr As String) As List(Of Part)

    Function Search(conditions As PartSearchModel) As IQueryable(Of Part)
    Function Create(part As Part) As Boolean
    Function Update(part As Part) As Boolean
End Interface
