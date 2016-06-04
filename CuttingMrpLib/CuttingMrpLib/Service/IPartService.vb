Public Interface IPartService
    Function FindById(id As String) As Part
    Function FuzzyById(id As String) As List(Of Part)
    Function GetParentParts(partNr As String) As List(Of Part)
End Interface
