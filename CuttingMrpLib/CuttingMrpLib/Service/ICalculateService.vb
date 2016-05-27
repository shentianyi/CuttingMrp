Public Interface ICalculateService
    Sub Start(quequeAddr As String, mrpSettings As CalculateSetting)
    Function Search(conditons As MRPSearchModel) As List(Of MrpRound)
End Interface
