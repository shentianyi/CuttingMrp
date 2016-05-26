Public Class RequirementStatistics
    Private _partNr As String
    Private _condition As String
    Private _sum As Double

    Public Property PartNr As String
        Get
            Return _partNr
        End Get
        Set(value As String)
            _partNr = value
        End Set
    End Property

    Public Property Condition As String
        Get
            Return _condition
        End Get
        Set(value As String)
            _condition = value
        End Set
    End Property

    Public Property SumOfQuantity As Double
        Get
            Return _sum
        End Get
        Set(value As Double)
            _sum = value
        End Set
    End Property
End Class
