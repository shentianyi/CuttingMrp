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
End Class