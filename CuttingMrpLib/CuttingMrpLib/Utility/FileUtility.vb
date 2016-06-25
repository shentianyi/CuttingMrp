Imports System.IO

Public Class FileUtility

    ''' <summary>
    ''' get all files from dir
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    Public Shared Function GetAllFilesFromDirectory(dir As String) As List(Of String)
        'Try
        Return Directory.GetFiles(dir.Trim).ToList
        ' Catch ex As Exception

        'Return Nothing
        ' End Try
    End Function

    ''' <summary>
    ''' check if file is open
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Public Shared Function IsFileOpen(fileName As String) As Boolean
        Try
            Using File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                Return True
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
