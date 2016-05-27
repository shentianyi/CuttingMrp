Public Interface IRequirementService
    Function Search(conditions As RequirementSearchModel) As IQueryable(Of Requirement)
    Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics)
    Function FindById(id As Integer) As Requirement
    Function DeleteById(id As Integer) As Boolean
    Function Update(stock As Requirement) As Boolean
End Interface
