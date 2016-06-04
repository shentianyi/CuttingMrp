Imports CuttingMrpLib
Imports Repository

Public Class StockSumRecordService

    Inherits ServiceBase
    Implements IStockSumRecordService
    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Sub Generate(dateTime As Date) Implements IStockSumRecordService.Generate

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As Repository(Of StockSumRecord) = New Repository(Of StockSumRecord)(context)
        Dim sumStockRep As Repository(Of SumOfStock) = New Repository(Of SumOfStock)(context)
        Dim sumStocks As List(Of SumOfStock) = sumStockRep.All.ToList
        Dim records As List(Of StockSumRecord) = New List(Of StockSumRecord)
        Dim prevDate As Date = dateTime.AddDays(-1)
        ' clean old data
        Dim oldRecords As List(Of StockSumRecord) = rep.GetTable.Where(Function(r) r.date.Equals(dateTime)).ToList
        For Each r In oldRecords
            rep.MarkForDeletion(r)
        Next

        For Each s In sumStocks
            ' find prev date part's stock
            Dim oldRecord As StockSumRecord = rep.FirstOrDefault(Function(r) r.partNr.Equals(s.partNr) And r.date.Equals(prevDate))
            Dim rate As Double = 0
            If oldRecord IsNot Nothing Then
                If s.SumOfStock.Value <> 0 Then
                    rate = (s.SumOfStock.Value - oldRecord.quantity) / s.SumOfStock.Value
                End If
            End If

            'If s.SumOfStock.HasValue And s.SumOfStock.Value >  0 Then
            records.Add(New StockSumRecord() With {.partNr = s.partNr,
                        .quantity = s.SumOfStock,
                        .date = dateTime,
                        .rate = rate})
            'End If
        Next

        rep.Inserts(records)
        rep.SaveAll()
    End Sub
End Class
