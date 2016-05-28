Partial Public Class ProcessOrder
    Public ReadOnly Property statusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.status, ProcessOrderStatus))
        End Get
    End Property

End Class
