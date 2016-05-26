Imports System.Data.Linq
Imports Repository
Public Class DataContext
    Implements IDataContextFactory

    Private _connStr As String
    Public Sub New(connStr As String)
        _connStr = connStr
    End Sub

    Private _dataContext As CuttingMrpDataContext

    Public ReadOnly Property Context As System.Data.Linq.DataContext Implements IDataContextFactory.Context
        Get
            If _dataContext Is Nothing Then
                _dataContext = New CuttingMrpDataContext(_connStr)
            End If
            Return _dataContext
        End Get
    End Property

    Public Sub SaveAll() Implements IDataContextFactory.SaveAll
        _dataContext.SubmitChanges()
    End Sub
End Class
