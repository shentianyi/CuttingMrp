﻿Partial Public Class ProcessOrder
    Public ReadOnly Property statusDisplay As String
        Get
            Return EnumUtility.GetDescription(DirectCast(Me.status, ProcessOrderStatus))
        End Get
    End Property

    Public ReadOnly Property canFinish As Boolean
        Get
            Return CanFinishStatus.Contains(Me.status)
        End Get
    End Property

    Public ReadOnly Property canDelete As Boolean
        Get
            Return CanDeleteStatus.Contains(Me.status)
        End Get
    End Property

    Public ReadOnly Property requirementCount As Integer
        Get
            Return Me.OrderDerivations.Count
        End Get
    End Property

    Public Shared CanFinishStatus As List(Of ProcessOrderStatus) = New List(Of ProcessOrderStatus) From {ProcessOrderStatus.Open}
    Public Shared CanDeleteStatus As List(Of ProcessOrderStatus) = New List(Of ProcessOrderStatus) From {ProcessOrderStatus.Open}

End Class