Partial Public Class Part
    Private _defaultBatchOrderTemplate As BatchOrderTemplate

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

    Public ReadOnly Property kanbanBatchQty As Double
        Get
            If Me.DefaultBatchOrderTemplate IsNot Nothing Then
                Return Me.DefaultBatchOrderTemplate.batchQuantity
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property kanbanBundleQty As Double
        Get
            If Me.DefaultBatchOrderTemplate IsNot Nothing Then
                Return Me.DefaultBatchOrderTemplate.bundle
            End If
            Return 0
        End Get
    End Property


    Public ReadOnly Property kanbanPosition As String
        Get
            If Me.DefaultBatchOrderTemplate IsNot Nothing Then
                Return If(Me.DefaultBatchOrderTemplate.remark1 Is Nothing, String.Empty, Me.DefaultBatchOrderTemplate.remark1)
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property DefaultBatchOrderTemplate As BatchOrderTemplate
        Get
            If Me._defaultBatchOrderTemplate Is Nothing Then
                Me._defaultBatchOrderTemplate = Me.BatchOrderTemplates.FirstOrDefault
            End If
            Return _defaultBatchOrderTemplate
        End Get
    End Property
End Class
