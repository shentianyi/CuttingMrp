Partial Public Class UnDoneStock
    Public ReadOnly Property sourceTypeDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.sourceType, PartType))
        End Get
    End Property

    Public ReadOnly Property stateDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.state, StockState))
        End Get
    End Property

End Class
