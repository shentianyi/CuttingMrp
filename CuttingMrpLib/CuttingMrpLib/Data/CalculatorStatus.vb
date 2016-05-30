Imports System.ComponentModel

Public Enum CalculatorStatus
    <Description("Waiting")>
    Waiting = 1000
    <Description("Running")>
    Running = 2000
    <Description("Cancel")>
    Cancel = 3000
    <Description("Finish")>
    Finish = 4000
End Enum
