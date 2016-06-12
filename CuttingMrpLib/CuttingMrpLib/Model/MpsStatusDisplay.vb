Partial Public Class MP
    Public ReadOnly Property statusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.status, MPSStatus))
        End Get
    End Property
End Class
