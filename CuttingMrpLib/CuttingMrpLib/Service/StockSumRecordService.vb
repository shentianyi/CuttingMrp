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
        Dim stocks As List(Of SumOfStock) = sumStockRep.All.ToList
        Dim records As List(Of StockSumRecord) = New List(Of StockSumRecord)
        ' clean old data
        If rep.FirstOrDefault(Function(r) r.date.Equals(dateTime)) IsNot Nothing Then
            Dim oldRecords As List(Of StockSumRecord) = rep.All.ToList
            For Each r In oldRecords
                rep.MarkForDeletion(r)
            Next
        End If

        For Each s In stocks
            If s.SumOfStock.HasValue And s.SumOfStock.Value > 0 Then
                records.Add(New StockSumRecord() With {.partNr = s.partNr, .quantity = s.SumOfStock, .date = dateTime})
            End If
        Next

        rep.Inserts(records)
        rep.SaveAll()
    End Sub
End Class
