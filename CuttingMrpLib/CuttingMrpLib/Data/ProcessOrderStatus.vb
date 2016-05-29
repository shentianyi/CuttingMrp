Imports System.ComponentModel
Public Enum ProcessOrderStatus
    <Description("Open")>
    Open = 1000
    <Description("Finish")>
    Finish = 2000
    <Description("SystemCancel")>
    SystemCancel = 3000
    <Description("ManualCancel")>
    ManualCancel = 4000
    <Description("Cancel")>
    Cancel = 5000
End Enum