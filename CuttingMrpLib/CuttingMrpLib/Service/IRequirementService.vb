Public Interface IRequirementService
    Function Search(conditions As RequirementSearchModel) As List(Of Requirement)
    Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics)
End Interface
