Imports CuttingMrpLib

Public Class UnDoneStockService
    Inherits ServiceBase
    Implements IUnDoneStockService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Create(undonestock As UnDoneStock) As Boolean Implements IUnDoneStockService.Create
        Throw New NotImplementedException()
    End Function

    Public Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock) Implements IUnDoneStockService.Search
        Dim reqRepo As IUnDoneStockRepository = New UnDoneStockRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function
End Class
