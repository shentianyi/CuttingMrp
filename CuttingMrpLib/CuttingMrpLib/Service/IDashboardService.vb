Public Interface IDashboardService
    Function GetPartStockDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem))
    Function GetPartCompleteRateDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem))
    Function GetPartTopRateDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem))
    Function GetStockReportDash(searchModel As DashboardSearchModel) As Dictionary(Of String, List(Of DashboardItem))
End Interface
