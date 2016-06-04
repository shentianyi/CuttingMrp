Partial Public Class StockMovement
    Public ReadOnly Property createdAtDisplay As String
        Get
            Return Me.createdAt.Value.ToString()
        End Get
    End Property

    Public ReadOnly Property fifoDisplay As String
        Get
            Return Me.fifo.ToString()
        End Get
    End Property

    Public ReadOnly Property typeDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.moveType, StockMoveType))
        End Get
    End Property

End Class
