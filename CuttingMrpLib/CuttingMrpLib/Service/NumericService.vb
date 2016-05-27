Imports CuttingMrpLib

Public Class NumericService
    Inherits ServiceBase
    Implements INumericService

    Public Sub New(conn As String)
        MyBase.New(conn)
    End Sub

    Public Function Generate(type As String) As String Implements INumericService.Generate
        Dim repo As INumericBuildRepository = New NumericDefinitionRepository(New DataContext(DBConn))
        Return repo.Generate(type)
    End Function

    Public Shared Function GenerateID(connstr As String, type As String) As String
        Dim svc As NumericService = New NumericService(connstr)
        Return svc.Generate(type)
    End Function
End Class
