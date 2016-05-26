Public Interface IRequirementRepository
    Function Search(searchModel As RequirementSearchModel) As IQueryable(Of Requirement)
    Function SearchStatistics(condition As RequirementStatisticsSearchModel) As IEnumerable(Of RequirementStatistics)

End Interface
