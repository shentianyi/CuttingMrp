
Public Class ProcessOrderSearchModel
    Inherits SearchModelBase
    Private _orderNr As String
    Private _sourceDoc As String
    Private _derivedFrom As String
    Private _proceeDateFrom As DateTime
    Private _proceeDateTo As DateTime
    Private _partNr As String
    Private _completeRateFrom As Double = -1
    Private _completeRateTo As Double = -1
    Private _status As Integer = -999

    Public Property OrderNr As String
        Get
            Return _orderNr
        End Get
        Set(value As String)
            _orderNr = value
        End Set
    End Property

    Public Property SourceDoc As String
        Get
            Return _sourceDoc
        End Get
        Set(value As String)
            _sourceDoc = value
        End Set
    End Property

    Public Property DerivedFrom As String
        Get
            Return _derivedFrom
        End Get
        Set(value As String)
            _derivedFrom = value
        End Set
    End Property

    Public Property ProceeDateFrom As DateTime
        Get
            Return _proceeDateFrom
        End Get
        Set(value As DateTime)
            _proceeDateFrom = value
        End Set
    End Property

    Public Property ProceeDateTo As DateTime
        Get
            Return _proceeDateTo
        End Get
        Set(value As DateTime)
            _proceeDateTo = value
        End Set
    End Property

    Public Property PartNr As String
        Get
            Return _partNr
        End Get
        Set(value As String)
            _partNr = value
        End Set
    End Property

    'Public Property SourceQuantity As Double
    '    Get
    '        Return _sourceQuantity
    '    End Get
    '    Set(value As Double)
    '        _sourceQuantity = value
    '    End Set
    'End Property

    'Public Property ActualQuantity As Double
    '    Get
    '        Return _actualQuantity
    '    End Get
    '    Set(value As Double)
    '        _actualQuantity = value
    '    End Set
    'End Property

    Public Property CompleteRateFrom As Double
        Get
            Return _completeRateFrom
        End Get
        Set(value As Double)
            _completeRateFrom = value
        End Set
    End Property
    Public Property CompleteRateTo As Double
        Get
            Return _completeRateTo
        End Get
        Set(value As Double)
            _completeRateTo = value
        End Set
    End Property

    Public Property Status As Integer
        Get
            Return _status
        End Get
        Set(value As Integer)
            _status = value
        End Set
    End Property
End Class
