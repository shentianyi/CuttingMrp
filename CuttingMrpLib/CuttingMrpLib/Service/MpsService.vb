Imports CuttingMrpLib

Public Class MpsService
    Inherits ServiceBase
    Implements IMpsService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(searchModel As MpsSeachModel) As IQueryable(Of MP) Implements IMpsService.Search
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As IMpsRepository = New MpsRepository(context)

        Return rep.Search(searchModel)
    End Function
End Class
