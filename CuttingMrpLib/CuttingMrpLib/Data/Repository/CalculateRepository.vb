Imports CuttingMrpLib
Imports Repository

Public Class CalculateRepository
    Inherits Repository(Of MrpRound)
    Implements ICalculateRepository
    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditions As MRPSearchModel) As IQueryable(Of MrpRound) Implements ICalculateRepository.Search
        If conditions Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim rounds As IQueryable(Of MrpRound) = _context.MrpRounds
            '   rounds = rounds.Where(Function(c) c.time >= conditions.TimeFrom And c.time <= conditions.TimeTo)

            If conditions.TimeFrom.HasValue Then
                rounds = rounds.Where(Function(c) c.time >= conditions.TimeFrom)
            End If

            If conditions.TimeTo.HasValue Then
                rounds = rounds.Where(Function(c) c.time <= conditions.TimeTo)
            End If

            If Not String.IsNullOrWhiteSpace(conditions.Launcher) Then
                rounds = rounds.Where(Function(c) c.launcher.Contains(conditions.Launcher))
            End If

            If String.IsNullOrWhiteSpace(conditions.MrpRoundId) = False Then
                rounds = rounds.Where(Function(c) c.mrpRound.Contains(conditions.MrpRoundId))
            End If

            If conditions.RunningStatus.HasValue Then
                rounds = rounds.Where(Function(c) c.runningStatus = conditions.RunningStatus)
            End If
            'rounds.Skip(conditions.PageSize * conditions.PageIndex)
            'rounds.Take(conditions.PageSize)
            Return rounds.OrderByDescending(Function(c) c.time)
        End If




    End Function
End Class
