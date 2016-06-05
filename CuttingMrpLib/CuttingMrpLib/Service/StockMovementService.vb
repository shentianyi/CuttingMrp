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

    'Public Function GetStockMovementInfo(conditions As StockMovementSearchModel) As StockMovementInfoModel Implements IStockMovementService.GetStockMovementInfo

    '    Dim info As StockMovementInfoModel = New StockMovementInfoModel
    '    Dim context = New DataContext(DBConn)
    '    Dim repo As StockMovementRepository = New StockMovementRepository(context)
    '    Dim q As IQueryable(Of StockMovement) = repo.Search(conditions)

    '    info.movementsCount = context.Context.GetTable(Of StockMovement).Where(Function(o) q.Select(Function(c) c.partNr).Contains(o.partNr)).Count

    '    Return info
    'End Function

End Class
