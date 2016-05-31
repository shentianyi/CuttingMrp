Partial Public Class Part
    Public ReadOnly Property partTypeDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.partType, PartType))
        End Get
    End Property

    Public ReadOnly Property productNr As String
        Get
            Return Me.partNr.Substring(0, 9)
        End Get
    End Property

    Public ReadOnly Property routeNr As String
        Get
            Return Me.partNr.Substring(9, Me.partNr.Length - 9)
        End Get
    End Property

    Public ReadOnly Property kanbanNrs As String
        Get
            Return String.Join("/", Me.BatchOrderTemplates.Select(Function(c) c.orderNr).ToArray())
        End Get
    End Property
End Class
