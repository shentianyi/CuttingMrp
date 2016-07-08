Public Class TimeUtility
    Public Shared Function Format(dateTime As Date) As String
        Return dateTime.ToString("yyyy-MM-dd HH:mm")
    End Function

End Class
