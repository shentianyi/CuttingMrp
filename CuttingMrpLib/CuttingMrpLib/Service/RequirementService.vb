Imports CuttingMrpLib
Imports Repository

Public Class RequirementService
    Inherits ServiceBase
    Implements IRequirementService
    Public Property dbString As String
 
    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(conditions As RequirementSearchModel) As IQueryable(Of Requirement) Implements IRequirementService.Search
        Try
            Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(Me.dbString))

            Return reqRepo.Search(conditions)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics) Implements IRequirementService.SearchStatistics
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(My.Settings.db))
        Return Nothing
    End Function

    Public Function FindById(id As Integer) As Requirement Implements IRequirementService.FindById
        Dim requirement As Requirement = Nothing
        Try
            Dim context As DataContext = New DataContext(Me.dbString)
            Dim rep As Repository(Of Requirement) = New Repository.Repository(Of Requirement)(context)
            requirement = rep.First(Function(s) s.id.Equals(id))
        Catch ex As Exception
            Dim s = ex.Message
        End Try
        Return requirement
    End Function


    Public Function DeleteById(id As Integer) As Boolean Implements IRequirementService.DeleteById
        Throw New NotImplementedException()
    End Function

    Public Function Update(stock As Requirement) As Boolean Implements IRequirementService.Update
        Throw New NotImplementedException()
    End Function
End Class
