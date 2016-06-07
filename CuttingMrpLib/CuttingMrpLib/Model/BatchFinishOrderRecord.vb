Public Class BatchFinishOrderRecord
    Private _fixOrderNr As String
    Private _partnr As String
    Private _amount As Double
    Private _prodtime As DateTime
    Private _line As Integer

    Public Property Id As String

    Public Property LineNr As Integer
        Get
            Return _line
        End Get
        Set(value As Integer)
            _line = value
        End Set
    End Property
    Public Property FixOrderNr As String
        Get
            Return _fixOrderNr
        End Get
        Set(value As String)
            _fixOrderNr = value
        End Set
    End Property

    Public Property PartNr As String
        Get
            Return _partnr
        End Get
        Set(value As String)
            _partnr = value
        End Set
    End Property

    Public Property Amount As Double
        Get
            Return _amount
        End Get
        Set(value As Double)
            _amount = value
        End Set
    End Property

    Public Property ProdTime As DateTime
        Get
            Return _prodtime
        End Get
        Set(value As DateTime)
            _prodtime = value
        End Set
    End Property


    Private _msgs As List(Of String)
    Public Property Msgs As List(Of String)
        Get
            If _msgs Is Nothing Then
                _msgs = New List(Of String)
            End If
            Return _msgs
        End Get
        Set(value As List(Of String))
            _msgs = value
        End Set
    End Property

    Private _warnings As List(Of String)
    Public Property Warnings As List(Of String)
        Get
            If _warnings Is Nothing Then
                _warnings = New List(Of String)
            End If
            Return _warnings
        End Get
        Set(value As List(Of String))
            _warnings = value
        End Set
    End Property
End Class
