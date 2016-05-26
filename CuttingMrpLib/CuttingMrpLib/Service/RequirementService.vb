Imports CuttingMrpLib
Imports Repository

Public Class RequirementService
    Implements IRequirementService

    Public Function Search(conditions As RequirementSearchModel) As List(Of Requirement) Implements IRequirementService.Search
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(My.Settings.db))
        Return reqRepo.Search(conditions)
    End Function

    Public Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics) Implements IRequirementService.SearchStatistics
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(My.Settings.db))

    End Function
End Class
