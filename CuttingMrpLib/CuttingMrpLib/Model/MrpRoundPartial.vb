Partial Public Class MrpRound
    Public ReadOnly Property runningStatusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.runningStatus, CalculatorStatus))
        End Get
    End Property


End Class
