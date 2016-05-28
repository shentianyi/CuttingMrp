
Public Class ProcessOrderSearchModel
    Inherits SearchModelBase


    Public Property OrderNr As String

    Public Property SourceDoc As String

    Public Property DerivedFrom As String

    Public Property ProceeDateFrom As DateTime?

    Public Property ProceeDateTo As DateTime?

    Public Property PartNr As String

    Public Property ActualQuantityFrom As Double?

    Public Property ActualQuantityTo As Double?


    Public Property CompleteRateFrom As Double?

    Public Property CompleteRateTo As Double?

    Public Property Status As Integer?

End Class
