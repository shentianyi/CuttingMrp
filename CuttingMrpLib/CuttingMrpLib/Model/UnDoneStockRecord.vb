Public Class UnDoneStockRecord

    Private _partnr As String
    Public Property PartNr As String
        Get
            Return _partnr
        End Get
        Set(value As String)
            _partnr = value
        End Set
    End Property

    Private _qunatity As Integer
    Public Property Quantity As Integer
        Get
            Return _qunatity
        End Get
        Set(value As Integer)
            _qunatity = value
        End Set
    End Property

    Private _kanbannr As String
    Public Property KanbanNr As String
        Get
            Return _kanbannr
        End Get
        Set(value As String)
            _kanbannr = value
        End Set
    End Property

    Private _sourcetype As Integer
    Public Property SourceType() As Integer
        Get
            Return _sourcetype
        End Get
        Set(ByVal value As Integer)
            _sourcetype = value
        End Set
    End Property

    Private _state As Integer
    Public Property State() As Integer
        Get
            Return _state
        End Get
        Set(ByVal value As Integer)
            _state = value
        End Set
    End Property

End Class
