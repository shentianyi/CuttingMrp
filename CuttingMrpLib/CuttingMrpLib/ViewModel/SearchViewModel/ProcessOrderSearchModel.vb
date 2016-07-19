
Public Class ProcessOrderSearchModel
    Inherits SearchModelBase

    Private isOrderByKanban As Boolean = True


    Public Property OrderNr As String

    Public Property SourceDoc As String

    Public Property DerivedFrom As String

    Public Property ProceeDateFrom As DateTime?

    Public Property ProceeDateTo As DateTime?

    Public Property PartNrAct As String

    Public Property PartNr As String

    Public Property ActualQuantityFrom As Double?

    Public Property ActualQuantityTo As Double?


    Public Property CompleteRateFrom As Double?

    Public Property CompleteRateTo As Double?

    Public Property Status As Integer?

    Public Property MrpRound As String

    Public Property KanbanNr As String

    Public Property CreateAtFrom As DateTime?

    Public Property CreateAtTo As DateTime?
    ''' <summary>
    ''' part type equals to kanban type
    ''' </summary>
    ''' <returns></returns>
    Public Property PartType As Integer?

    Public Property IsOrderyKanban As Boolean?
        Get
            Return Me.isOrderByKanban
        End Get
        Set(value As Boolean?)
            If value.HasValue Then
                Me.isOrderByKanban = value
            End If
        End Set
    End Property


End Class
