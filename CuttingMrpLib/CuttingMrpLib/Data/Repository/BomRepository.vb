Imports CuttingMrpLib
Imports Repository

Public Class BomRepository
    Inherits Repository.Repository(Of BOM)
    Implements IBomRepository

    Private _context As CuttingMrpDataContext

    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditions As BomSearchModel) As IQueryable(Of BOM) Implements IBomRepository.Search
        If conditions IsNot Nothing Then

            Dim boms As IQueryable(Of BOM) = _context.BOMs

            If Not String.IsNullOrWhiteSpace(conditions.ID) Then
                boms = boms.Where(Function(c) c.id.Contains(conditions.ID.Trim()))
            End If

            If Not String.IsNullOrWhiteSpace(conditions.PartNr) Then
                boms = boms.Where(Function(c) c.partNr.Contains(conditions.PartNr.Trim()))
            End If

            If conditions.VFFrom.HasValue Then
                boms = boms.Where(Function(c) c.validFrom >= conditions.VFFrom)
            End If

            If conditions.VFTo.HasValue Then
                boms = boms.Where(Function(c) c.validFrom <= conditions.VFTo)
            End If

            If conditions.VTFrom.HasValue Then
                boms = boms.Where(Function(c) c.validTo >= conditions.VTFrom)
            End If

            If conditions.VTTo.HasValue Then
                boms = boms.Where(Function(c) c.validTo <= conditions.VTTo)
            End If

            Return boms.OrderBy(Function(c) c.id)
        End If

        Return Nothing
    End Function
End Class
