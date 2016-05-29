Imports CuttingMrpLib
Imports Repository

Public Class MrpRoundRepository
    Inherits Repository(Of Stock)
    Implements IMrpRoundRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = Me._dataContextFactory.Context
    End Sub

    Public Function GetRecents(limit As Integer) As List(Of MrpRound) Implements IMrpRoundRepository.GetRecents
        Return Me._context.MrpRounds.OrderByDescending(Function(m) m.time).Take(limit).ToList
    End Function
End Class
