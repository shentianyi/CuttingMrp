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
        Return rep.First(Function(p) p.partNr.Equals(id))
    End Function
End Class
