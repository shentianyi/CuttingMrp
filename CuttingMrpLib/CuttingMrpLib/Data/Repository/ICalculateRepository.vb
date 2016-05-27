Public Interface ICalculateRepository
    Function Search(conditions As MRPSearchModel) As List(Of MrpRound)
End Interface
