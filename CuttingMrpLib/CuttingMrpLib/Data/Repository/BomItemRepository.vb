Imports CuttingMrpLib
Imports Repository

Public Class BomItemRepository
    Inherits Repository.Repository(Of BomItem)
    Implements IBomItemRepository

    Private _context As CuttingMrpDataContext

    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(condition As BomItemSearchModel) As IQueryable(Of BomItem) Implements IBomItemRepository.Search

        If condition IsNot Nothing Then

            Dim bomItem As IQueryable(Of BomItem) = _context.BomItems

            If Not String.IsNullOrWhiteSpace(condition.ComponentId) Then
                bomItem = bomItem.Where(Function(c) c.componentId.Contains(condition.ComponentId.Trim()))
            End If

            If condition.VFFrom.HasValue Then
                bomItem = bomItem.Where(Function(c) c.validFrom >= condition.VFFrom)
            End If

            If condition.VFTo.HasValue Then
                bomItem = bomItem.Where(Function(c) c.validFrom <= condition.VFTo)
            End If

            If condition.VTFrom.HasValue Then
                bomItem = bomItem.Where(Function(c) c.validTo >= condition.VTFrom)
            End If

            If condition.VTTO.HasValue Then
                bomItem = bomItem.Where(Function(c) c.validTo <= condition.VTTO)
            End If

            If Not String.IsNullOrWhiteSpace(condition.BomId) Then
                bomItem = bomItem.Where(Function(c) c.bomId.Contains(condition.BomId.Trim()))
            End If

            Return bomItem.OrderBy(Function(c) c.id)
        End If
        Return Nothing
    End Function
End Class
