Public Interface IDashboardService
    Function GetPartStockDash(searchModel As DashboardSearchModel) As List(Of DashboardItem)
    Function GetPartCompleteRateDash(searchModel As DashboardSearchModel) As List(Of DashboardItem)

End Interface
