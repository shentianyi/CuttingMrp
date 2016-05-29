Public Interface ICalculateService
    Sub Start(quequeAddr As String, mrpSettings As CalculateSetting)
    Function Search(conditons As MRPSearchModel) As IQueryable(Of MrpRound)
End Interface
