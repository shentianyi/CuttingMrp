Imports CuttingMrpLib
Imports Repository

Public Class StockMovementService
    Inherits ServiceBase
    Implements IStockMovementService


    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(searchModel As StockMovementSearchModel) As IQueryable(Of StockMovement) Implements IStockMovementService.Search
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As IStockMovementRepository = New StockMovementRepository(context)

        Return rep.Search(searchModel)
    End Function

End Class
