Imports Repository

Public Class ProcessOrderRepository
    Inherits Repository.Repository(Of ProcessOrder)
    Implements IProcessOrderRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub


End Class
