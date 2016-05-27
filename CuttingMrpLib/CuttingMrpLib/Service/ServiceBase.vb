Public Class ServiceBase
    Public Sub New(db As String)
        _db = db
    End Sub
    Private _db As String
    Public ReadOnly Property DBConn As String
        Get
            Return _db
        End Get
    End Property


End Class
