Public Interface IPartService
    Function FindById(id As String) As Part
    Function FuzzyById(id As String) As List(Of Part)
End Interface
