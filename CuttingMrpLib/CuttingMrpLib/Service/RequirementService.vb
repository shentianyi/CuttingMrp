Imports CuttingMrpLib
Imports Repository

Public Class RequirementService
    Inherits ServiceBase
    Implements IRequirementService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub
    Public Function Search(conditions As RequirementSearchModel) As List(Of Requirement) Implements IRequirementService.Search
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(DBConn))
        Return reqRepo.Search(conditions).ToList
    End Function

    Public Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics) Implements IRequirementService.SearchStatistics
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(DBConn))
        Return reqRepo.SearchStatistics(condition).ToList
    End Function
End Class
