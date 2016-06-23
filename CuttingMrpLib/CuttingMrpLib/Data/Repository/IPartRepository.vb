Public Interface IPartRepository
    Function GetParents(partNr As String, validFrom As DateTime, validTo As DateTime) As List(Of Part)

    Function Search(conditions As PartSearchModel) As IQueryable(Of Part)
End Interface
