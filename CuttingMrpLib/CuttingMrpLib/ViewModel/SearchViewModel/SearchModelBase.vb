Public Class SearchModelBase
    Private _pageSize As Integer
    Private _pageindex As Integer

    Public Property PageSize As Integer
        Get
            Return _pageSize
        End Get
        Set(value As Integer)
            _pageSize = value
        End Set
    End Property

    Public Property PageIndex As Integer
        Get
            Return _pageindex
        End Get
        Set(value As Integer)
            _pageindex = value
        End Set
    End Property


End Class
