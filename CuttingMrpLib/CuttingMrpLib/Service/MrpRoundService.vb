Imports CuttingMrpLib
Imports Repository

Public Class MrpRoundService
    Inherits ServiceBase
    Implements IMrpRoundService


    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function GetRecents(limit As Integer) As List(Of MrpRound) Implements IMrpRoundService.GetRecents
        Dim rep As IMrpRoundRepository = New MrpRoundRepository(New DataContext(Me.DBConn))
        Return rep.GetRecents(limit)
    End Function
End Class
