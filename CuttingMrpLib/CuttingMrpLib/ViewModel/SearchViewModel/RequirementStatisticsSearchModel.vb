Public Class RequirementStatisticsSearchModel
    Inherits SearchModelBase

    Private typs() As String = {"DERIVEDTYPE", "REQUIREDTIME_D", "REQUIREDTIME_W", "REQUIREDTIME_M", "ORDERTIME_D",
        "ORDERTIME_W", "ORDERTIME_M"}

    Private _statisticsType As String
    Public Property StatisticsType As String
        Get
            Return _statisticsType
        End Get
        Set(value As String)
            If Not typs.Contains(value) Then
                Throw New Exception("search type not exist")
            End If
            _statisticsType = value
        End Set
    End Property

End Class
