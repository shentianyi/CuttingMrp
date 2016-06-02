Imports CuttingMrpLib

Public Class CapacityPlanService
    Implements ICapacityPlanService

    Public Sub ChangeCapacityPlan(plan As CapacityPlan, ByVal shouldRearrange As Boolean, shouldCreate As Boolean) Implements ICapacityPlanService.ChangeCapacityPlan
        If plan Is Nothing Then
            Throw New ArgumentNullException
        End If

    End Sub

    Public Sub RearrangePlan(ByVal unitId As String)


    End Sub
End Class
