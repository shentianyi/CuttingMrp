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

    Public Function Search(conditions As MRPSearchModel) As List(Of MrpRound) Implements ICalculateRepository.Search
        If conditions Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim rounds As IQueryable(Of MrpRound) = _context.MrpRounds
            rounds = rounds.Where(Function(c) c.time >= conditions.TimeFrom And c.time <= conditions.TimeTo)
            If String.IsNullOrEmpty(conditions.Launcher) = False Then
                rounds = rounds.Where(Function(c) c.launcher Like conditions.Launcher)

            End If
            If String.IsNullOrEmpty(conditions.MrpRoundId) = False Then
                rounds = rounds.Where(Function(c) c.mrpRound Like conditions.MrpRoundId)
            End If
            If conditions.RunningStatus <> -999 Then
                rounds = rounds.Where(Function(c) c.runningStatus = conditions.RunningStatus)
            End If
            rounds.Skip(conditions.PageSize * conditions.PageIndex)
            rounds.Take(conditions.PageSize)
            Return rounds.ToList
        End If




    End Function
End Class
