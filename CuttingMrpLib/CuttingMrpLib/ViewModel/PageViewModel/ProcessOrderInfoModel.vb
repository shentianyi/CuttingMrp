Public Class ProcessOrderInfoModel
    Public Property latestOrder As ProcessOrder

    Public Property oldestOrder As ProcessOrder

    Public Property processOrderCount As Integer

    Public Property requirementCount As Integer

    Public ReadOnly Property latestOrderDate As DateTime?
        Get
            If Me.latestOrder IsNot Nothing Then
                Return Me.latestOrder.proceeDate
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property oldestOrderDate As DateTime?
        Get
            If Me.oldestOrder IsNot Nothing Then
                Return Me.oldestOrder.proceeDate
            Else
                Return Nothing
            End If
        End Get
    End Property
End Class
