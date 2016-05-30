Imports CuttingMrpLib.EnumItem

Public Class RequirementStatisticsSearchModel
    Inherits SearchModelBase

    Private types() As String = {
        "DERIVEDTYPE", "REQUIREDTIME_D",
        "REQUIREDTIME_W", "REQUIREDTIME_M", "ORDERTIME_D",
        "ORDERTIME_W", "ORDERTIME_M"}


    Public Shared typeList As List(Of EnumItem) = New List(Of EnumItem)(New EnumItem() {
                                                                 New EnumItem() With {.Text = "DrivedType", .Value = "DERIVEDTYPE"},
                                                                 New EnumItem() With {.Text = "RequireTime Day", .Value = "REQUIREDTIME_D"},
                                                                 New EnumItem() With {.Text = "RequireTime Week", .Value = "REQUIREDTIME_W"},
                                                                 New EnumItem() With {.Text = "RequireTime Month", .Value = "REQUIREDTIME_M"},
                                                                 New EnumItem() With {.Text = "OrderTime Day", .Value = "ORDERTIME_D"},
                                                                 New EnumItem() With {.Text = "OrderTime Week", .Value = "ORDERTIME_W"},
                                                                 New EnumItem() With {.Text = "OrderTime Month", .Value = "ORDERTIME_M"}
                                                                 })

    Private _statisticsType As String
    Public Property StatisticsType As String
        Get
            Return _statisticsType
        End Get
        Set(value As String)
            If Not String.IsNullOrWhiteSpace(value) Then
                If Not types.Contains(value) Then
                    Throw New Exception("search type not exist")
                End If
            End If
            _statisticsType = value
        End Set
    End Property

End Class
