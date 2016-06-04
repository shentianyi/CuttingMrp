Imports CuttingMrpLib
Imports Repository

Public Class DashboardService
    Inherits ServiceBase
    Implements IDashboardService


    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function GetPartCompleteRateDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem)) Implements IDashboardService.GetPartCompleteRateDash
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of AvgOfCompleteRate) = New Repository(Of AvgOfCompleteRate)(context)
        Dim rates As List(Of AvgOfCompleteRate) = rep.FindAll(Function(c) c.partNr.Equals(searchModel.PartNr) And c.proceeDate >= searchModel.DateFrom And c.proceeDate <= searchModel.DateTo).ToList
        Dim items As List(Of DashboardItem) = New List(Of DashboardItem)
        Dim d As DateTime = searchModel.DateFrom

        For Each r In rates
            items.Add(New DashboardItem() With {.XValue = r.proceeDate.Date.ToString, .YValue = r.rate})
        Next

        'While d <= searchModel.DateTo
        '    Dim rate = rates.Where(Function(r) r.proceeDate.Equals(d)).FirstOrDefault
        '    items.Add(New DashboardItem() With {.XValue = d.Date.ToString, .YValue = If(rate Is Nothing, 0, rate.rate)})
        '    d = d.AddDays(1)
        'End While

        ' Return items

        Dim dic As Dictionary(Of String, List(Of DashboardItem)) = New Dictionary(Of String, List(Of DashboardItem))
        dic.Add(searchModel.PartNr, items)
        Return dic
    End Function

    Public Function GetPartStockDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem)) Implements IDashboardService.GetPartStockDash
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of StockSumRecord) = New Repository(Of StockSumRecord)(context)
        Dim stocks As List(Of StockSumRecord) = rep.FindAll(Function(c) c.partNr.Equals(searchModel.PartNr) And c.date >= searchModel.DateFrom And c.date <= searchModel.DateTo).ToList
        Dim items As List(Of DashboardItem) = New List(Of DashboardItem)
        Dim d As DateTime = searchModel.DateFrom
        While d <= searchModel.DateTo
            Dim stock = stocks.Where(Function(s) s.date.Equals(d)).FirstOrDefault
            items.Add(New DashboardItem() With {.XValue = d.Date.ToString,
                      .YValue = If(stock Is Nothing, 0, stock.quantity),
                      .YValueRate = If(stock Is Nothing, 0, stock.rate)})
            d = d.AddDays(1)
        End While
        Dim dic As Dictionary(Of String, List(Of DashboardItem)) = New Dictionary(Of String, List(Of DashboardItem))
        dic.Add(searchModel.PartNr, items)
        Return dic
    End Function

    Public Function GetPartTopRateDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem)) Implements IDashboardService.GetPartTopRateDash
        Dim dic As Dictionary(Of String, List(Of DashboardItem)) = New Dictionary(Of String, List(Of DashboardItem))
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of StockSumRecord) = New Repository(Of StockSumRecord)(context)

        Dim topPartNrs = rep.GetTable.Where(Function(r) r.date.Equals(searchModel.DateTo)).OrderByDescending(Function(r) Math.Abs(r.rate.Value)).Take(searchModel.Top).Select(Function(r) r.partNr).ToList
        For Each partNr In topPartNrs
            Dim pdic As Dictionary(Of String, List(Of DashboardItem)) = GetPartStockDash(New DashboardSearchModel() With {.PartNr = partNr, .DateFrom = searchModel.DateFrom, .DateTo = searchModel.DateTo})
            dic.Add(partNr, pdic.Values.First)
        Next

        Return dic
    End Function
End Class
