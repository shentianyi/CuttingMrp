Imports CuttingMrpLib
Imports Repository

Public Class BatchOrderTemplateRepository
    Inherits Repository.Repository(Of BatchOrderTemplate)
    Implements IBatchOrderTemplateRepository

    Private _context As CuttingMrpDataContext

    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(condition As BatchOrderTemplateSearchModel) As IQueryable(Of BatchOrderTemplate) Implements IBatchOrderTemplateRepository.Search
        If condition IsNot Nothing Then

            Dim batchOrderTempalte As IQueryable(Of BatchOrderTemplate) = _context.BatchOrderTemplates

            If Not String.IsNullOrWhiteSpace(condition.OrderNr) Then
                batchOrderTempalte = batchOrderTempalte.Where(Function(c) c.orderNr.Contains(condition.OrderNr.Trim()))
            End If

            If Not String.IsNullOrWhiteSpace(condition.PartNr) Then
                batchOrderTempalte = batchOrderTempalte.Where(Function(c) c.partNr.Contains(condition.PartNr.Trim()))
            End If

            If condition.Type.HasValue Then
                batchOrderTempalte = batchOrderTempalte.Where(Function(c) c.type.Equals(condition.Type))
            End If

            If Not String.IsNullOrWhiteSpace(condition.Remark1) Then
                batchOrderTempalte = batchOrderTempalte.Where(Function(c) c.remark1.Contains(condition.Remark1.Trim()))
            End If

            Return batchOrderTempalte.OrderBy(Function(c) c.orderNr)
        End If

        Return Nothing
    End Function
End Class
