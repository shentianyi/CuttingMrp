Public Interface ICalculateRepository
    Function Search(conditions As MRPSearchModel) As IQueryable(Of MrpRound)
End Interface
