Imports CuttingMrpLib
Imports Repository

Public Class ProcessOrderRepository
    Inherits Repository.Repository(Of ProcessOrder)
    Implements IProcessOrderRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder) Implements IProcessOrderRepository.Search

        If conditions IsNot Nothing Then
            Dim table As IQueryable(Of ProcessOrder) = _context.ProcessOrders
            table = table.Where(Function(c) c.proceeDate >= conditions.ProceeDateFrom And c.proceeDate <= conditions.ProceeDateTo)

            If Not String.IsNullOrEmpty(conditions.OrderNr) Then
                table = table.Where(Function(c) c.orderNr Like conditions.OrderNr)
            End If
            If Not String.IsNullOrEmpty(conditions.DerivedFrom) Then
                table = table.Where(Function(c) c.derivedFrom = conditions.DerivedFrom)
            End If
            If Not String.IsNullOrEmpty(conditions.SourceDoc) Then
                table = table.Where(Function(c) c.sourceDoc Like conditions.SourceDoc)
            End If
            If Not String.IsNullOrEmpty(conditions.PartNr) Then
                table = table.Where(Function(c) c.partNr Like conditions.PartNr)
            End If
            If conditions.Status <> -999 Then
                table = table.Where(Function(c) c.status = conditions.Status)
            End If
            If conditions.CompleteRateFrom >= 0 Then
                table = table.Where(Function(c) c.completeRate >= conditions.CompleteRateFrom)
            End If
            If conditions.CompleteRateTo >= 0 Then
                table = table.Where(Function(c) c.completeRate <= conditions.CompleteRateTo)
            End If

            table.Skip(conditions.PageSize * conditions.PageIndex)
            table.Take(conditions.PageSize)
            Return table
        Else
            Throw New ArgumentNullException
        End If

    End Function
End Class
