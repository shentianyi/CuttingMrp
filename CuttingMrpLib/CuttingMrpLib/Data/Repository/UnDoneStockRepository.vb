Imports CuttingMrpLib
Imports Repository

Public Class UnDoneStockRepository
    Inherits Repository.Repository(Of UnDoneStock)
    Implements IUnDoneStockRepository

    Private _context As CuttingMrpDataContext

    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock) Implements IUnDoneStockRepository.Search
        If conditions IsNot Nothing Then
            Dim undonestocks As IQueryable(Of UnDoneStock) = _context.UnDoneStock

            If Not String.IsNullOrWhiteSpace(conditions.PartNr) Then
                undonestocks = undonestocks.Where(Function(c) c.partNr.Contains(conditions.PartNr.Trim()))
            End If

            If conditions.SourceType.HasValue Then
                undonestocks = undonestocks.Where(Function(c) c.sourceType.Equals(conditions.SourceType))
            End If

            If conditions.State.HasValue Then
                undonestocks = undonestocks.Where(Function(c) c.state.Equals(conditions.State))
            End If

            Return undonestocks.OrderBy(Function(c) c.id)

        End If

        Return Nothing

    End Function
End Class
