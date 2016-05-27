Partial Public Class Requirement
    Public ReadOnly Property statusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.status, RequirementStatus))
        End Get
    End Property


End Class
