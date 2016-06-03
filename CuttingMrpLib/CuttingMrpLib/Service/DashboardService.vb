Imports CuttingMrpLib
Imports Repository

Public Class DashboardService
    Inherits ServiceBase
    Implements IDashboardService


    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function GetPartCompleteRateDash(searchModel As DashboardSearchModel) As List(Of DashboardItem) Implements IDashboardService.GetPartCompleteRateDash
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of AvgOfCompleteRate) = New Repository(Of AvgOfCompleteRate)(context)
        Dim rates As List(Of AvgOfCompleteRate) = rep.FindAll(Function(c) c.partNr.Equals(searchModel.PartNr) And c.proceeDate >= searchModel.DateFrom And c.proceeDate <= searchModel.DateTo).ToList
        Dim items As List(Of DashboardItem) = New List(Of DashboardItem)
        Dim d As DateTime = searchModel.DateFrom

        For Each r In rates
            items.Add(New DashboardItem() With {.XValue = r.proceeDate.Date.ToString, .YValue = r.rate})
        Next

        Return items
    End Function

    Public Function GetPartStockDash(searchModel As DashboardSearchModel) As List(Of DashboardItem) Implements IDashboardService.GetPartStockDash
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of StockSumRecord) = New Repository(Of StockSumRecord)(context)
        Dim stocks As List(Of StockSumRecord) = rep.FindAll(Function(c) c.partNr.Equals(searchModel.PartNr) And c.date >= searchModel.DateFrom And c.date <= searchModel.DateTo).ToList
        Dim items As List(Of DashboardItem) = New List(Of DashboardItem)
        Dim d As DateTime = searchModel.DateFrom
        While d <= searchModel.DateTo
            Dim stock = stocks.Where(Function(s) s.date.Equals(d)).FirstOrDefault
            items.Add(New DashboardItem() With {.XValue = d.Date.ToString, .YValue = If(stock Is Nothing, 0, stock.ToString)})
            d = d.AddDays(1)
        End While
        Return items
    End Function
End Class
