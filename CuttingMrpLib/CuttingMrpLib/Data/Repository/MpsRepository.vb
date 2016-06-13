Imports CuttingMrpLib
Imports Repository

Public Class MpsRepository
    Inherits Repository.Repository(Of MP)
    Implements IMpsRepository

    Private _context As CuttingMrpDataContext

    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditons As MpsSeachModel) As IQueryable(Of MP) Implements IMpsRepository.Search
        If conditons IsNot Nothing Then
            Dim mps As IQueryable(Of MP) = _context.MPs

            If Not String.IsNullOrWhiteSpace(conditons.PartNr) Then
                mps = mps.Where(Function(c) c.partnr.Contains(conditons.PartNr.Trim()))
            End If

            If conditons.OrderedDateFrom.HasValue Then
                mps = mps.Where(Function(c) c.orderedDate >= conditons.OrderedDateFrom)
            End If

            If conditons.OrderedDateTo.HasValue Then
                mps = mps.Where(Function(c) c.orderedDate <= conditons.OrderedDateTo)
            End If

            If conditons.RequiredDateFrom.HasValue Then
                mps = mps.Where(Function(c) c.requiredDate >= conditons.RequiredDateFrom)
            End If

            If conditons.RequiredDateTo.HasValue Then
                mps = mps.Where(Function(c) c.requiredDate <= conditons.RequiredDateTo)
            End If

            If conditons.Status.HasValue Then
                mps = mps.Where(Function(c) c.status.Equals(conditons.Status))
            End If
            Return mps.OrderByDescending(Function(c) c.partnr)
        End If
        Return Nothing
    End Function
End Class
