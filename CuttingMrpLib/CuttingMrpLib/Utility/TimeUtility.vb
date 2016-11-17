Public Class TimeUtility
    Public Shared Function Format(dateTime As Date) As String
        Return dateTime.ToString("yyyy-MM-dd HH:mm:sss")
    End Function

End Class
