Imports CuttingMrpLib
Imports Repository

Public Class PartService
    Inherits ServiceBase
    Implements IPartService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function FindById(id As String) As Part Implements IPartService.FindById
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of Part) = New Repository.Repository(Of Part)(context)
        Return rep.FirstOrDefault(Function(p) p.partNr.Equals(id))
    End Function

    Public Function FuzzyById(id As String) As List(Of Part) Implements IPartService.FuzzyById
        Dim context As DataContext = New DataContext(Me.DBConn)
        Return context.Context.GetTable(Of Part).Where(Function(p) p.partNr.Contains(id)).OrderBy(Function(p) p.partNr).Take(20).ToList
    End Function

    Public Function GetParentParts(partNr As String) As List(Of Part) Implements IPartService.GetParentParts
        Return New PartRepository(New DataContext(Me.DBConn)).GetParents(partNr, DateTime.Now, DateTime.Now)
    End Function

    Public Function Search(conditions As PartSearchModel) As IQueryable(Of Part) Implements IPartService.Search
        Dim reqRepo As IPartRepository = New PartRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function

    Public Function Create(part As Part) As Boolean Implements IPartService.Create
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As PartRepository = New PartRepository(context)

        Dim upart As Part = rep.FirstOrDefault(Function(s) s.partNr.Equals(part.partNr))

        If (upart Is Nothing) Then
            rep.MarkForAdd(part)
            rep.SaveAll()

            result = True
        End If

        Return result
    End Function

    Public Function Update(part As Part) As Boolean Implements IPartService.Update
        Dim result As Boolean = False

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As PartRepository = New PartRepository(context)
        Dim upart As Part = rep.FirstOrDefault(Function(s) s.partNr.Equals(part.partNr))

        If (upart IsNot Nothing) Then
            upart.partType = part.partType
            upart.partDesc = part.partDesc
            upart.partStatus = part.partStatus
            upart.moq = part.moq
            upart.spq = part.spq

            rep.SaveAll()

            result = True
        End If
        Return result
    End Function
End Class