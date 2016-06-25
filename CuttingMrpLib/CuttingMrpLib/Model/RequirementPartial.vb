Partial Public Class Requirement
    Private _reduceQuantity As Double?

    Public ReadOnly Property statusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.status, RequirementStatus))
        End Get
    End Property

    Public Property reduceQuantity As Double
        Get
            If _reduceQuantity Is Nothing Then
                _reduceQuantity = quantity
            End If
            Return _reduceQuantity
        End Get
        Set(value As Double)
            _reduceQuantity = value
        End Set
    End Property

End Class
