Imports CuttingMrpLib
Imports Repository

Public Class RequirementService
    Inherits ServiceBase
    Implements IRequirementService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(conditions As RequirementSearchModel) As IQueryable(Of Requirement) Implements IRequirementService.Search
        Try
            Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(Me.DBConn))

            Return reqRepo.Search(conditions)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function SearchStatistics(condition As RequirementStatisticsSearchModel) As List(Of RequirementStatistics) Implements IRequirementService.SearchStatistics
        Dim reqRepo As RequirementRepository = New RequirementRepository(New DataContext(Me.DBConn))
        Dim v = reqRepo.SearchStatistics(condition)
        Return v.ToList
    End Function

    Public Function FindById(id As Integer) As Requirement Implements IRequirementService.FindById
        Dim requirement As Requirement = Nothing
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Requirement) = New Repository.Repository(Of Requirement)(context)
            requirement = rep.FirstOrDefault(Function(s) s.id.Equals(id))
        Catch ex As Exception
            Dim s = ex.Message
        End Try
        Return requirement
    End Function


    Public Function DeleteById(id As Integer) As Boolean Implements IRequirementService.DeleteById
        Dim result As Boolean = False
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Requirement) = New Repository.Repository(Of Requirement)(context)
            Dim requirement As Requirement = rep.FirstOrDefault(Function(s) s.id.Equals(id))
            If (requirement IsNot Nothing) Then
                rep.MarkForDeletion(requirement)
                context.SaveAll()
                result = True
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function Update(requirement As Requirement) As Boolean Implements IRequirementService.Update
        Dim result As Boolean = False
        Try
            Dim context As DataContext = New DataContext(Me.DBConn)
            Dim rep As Repository(Of Requirement) = New Repository.Repository(Of Requirement)(context)
            Dim urequirement As Requirement = rep.FirstOrDefault(Function(s) s.id.Equals(requirement.id))
            If (urequirement IsNot Nothing) Then
                urequirement.partNr = requirement.partNr
                urequirement.orderedDate = requirement.orderedDate
                urequirement.requiredDate = requirement.requiredDate
                urequirement.quantity = requirement.quantity
                urequirement.status = requirement.status
                urequirement.derivedFrom = requirement.derivedFrom
                urequirement.derivedType = requirement.derivedType


                context.SaveAll()
                result = True
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function
End Class
