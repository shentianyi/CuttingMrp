Public Class CalculateSetting
    Private _roundId As String
    Private _reserveTypes As List(Of String)
    Public MergeMethodType() As String = {"DAY", "WEEK", "MONTH", "YEAR"}
    Public OrderTypes() As String = {"FIX", "ACTUAL"}

    Public Property ReservedType As List(Of String)
        Get
            If _reserveTypes Is Nothing Then
                _reserveTypes = New List(Of String)
            End If
            Return _reserveTypes
        End Get
        Set(value As List(Of String))
            _reserveTypes = value
        End Set
    End Property
    Public Property RoundId As String
        Get
            Return _roundId
        End Get
        Set(value As String)
            _roundId = value
        End Set
    End Property
    ''' <summary>
    ''' DAY, WEEK, MONTH, YEAR
    ''' </summary>
    Private _mergeMethod As String
    Public Property MergeMethod As String
        Get
            Return _mergeMethod
        End Get
        Set(value As String)
            If MergeMethodType.Contains(value) = False Then
                Throw New Exception("Merge method not supported")
            Else
                _mergeMethod = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' FIX,ACTUAL
    ''' </summary>
    Private _orderType As String
    Public Property OrderType As String
        Get
            Return _orderType
        End Get
        Set(value As String)
            If OrderTypes.Contains(value) = False Then
                Throw New Exception("order method not supported")
            Else
                _orderType = value
            End If
        End Set
    End Property

End Class
