<Serializable>
Public Class CalculateSetting
    Private _taskType As String = "MRP"
    Private _roundId As String
    Private _reserveTypes As List(Of String)
    Private _mergeMethod As MergeMethod
    Public MergeMethodType() As String = {"DAY", "WEEK", "MONTH", "YEAR"}
    Public OrderTypes() As String = {"FIX", "ACTUAL"}

    ''' <summary>
    ''' MRP,BF
    ''' </summary>
    ''' <returns></returns>
    Public Property TaskType As String
        Get
            Return _taskType
        End Get
        Set(value As String)
            _taskType = value
        End Set
    End Property
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
    Public Property MergeMethod As MergeMethod
        Get
            Return _mergeMethod
        End Get
        Set(value As MergeMethod)
            If MergeMethodType.Contains(value.MergeType) = False Then
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

Public Class MergeMethod
    Private _type As String
    Private _firstDay As DateTime
    Private _count As Integer

    Public Property MergeType As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    Public Property FirstDay As DateTime
        Get
            Return _firstDay
        End Get
        Set(value As DateTime)
            _firstDay = value
        End Set
    End Property

    Public Property Count As Integer
        Get
            Return _count
        End Get
        Set(value As Integer)
            _count = value
        End Set
    End Property
End Class
