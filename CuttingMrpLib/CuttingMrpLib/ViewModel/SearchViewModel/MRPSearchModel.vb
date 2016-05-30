Public Class MRPSearchModel
    Inherits SearchModelBase

    Private _mrpRoundId As String
    Private _runningStatus As Integer?
    Private _timefrom As DateTime?
    Private _timeto As DateTime?
    Private _launcher As String

    Public Property MrpRoundId As String
        Get
            Return _mrpRoundId
        End Get
        Set(value As String)
            _mrpRoundId = value
        End Set
    End Property

    Public Property RunningStatus As Integer?
        Get
            Return _runningStatus
        End Get
        Set(value As Integer?)
            _runningStatus = value
        End Set
    End Property

    Public Property TimeFrom As DateTime?
        Get
            Return _timefrom
        End Get
        Set(value As DateTime?)
            _timefrom = value
        End Set
    End Property

    Public Property TimeTo As DateTime?
        Get
            Return _timeto
        End Get
        Set(value As DateTime?)
            _timeto = value
        End Set
    End Property

    Public Property Launcher As String
        Get
            Return _launcher
        End Get
        Set(value As String)
            _launcher = value
        End Set
    End Property
End Class
