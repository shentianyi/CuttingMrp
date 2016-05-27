Imports System.Reflection
Imports System.ComponentModel

Public Class EnumUtility
    Public Shared Function GetDescription(value As [Enum]) As String
        Dim desc As String = String.Empty
        Dim info As FieldInfo = value.GetType().GetField(value.ToString())
        Dim attributes() As DescriptionAttribute = Nothing
        Try
            attributes = DirectCast(info.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
        Catch ex As Exception

        End Try
        If attributes IsNot Nothing AndAlso attributes.Length > 0 Then
            desc = attributes(0).Description
        End If
        Return desc
    End Function

    Public Shared Function GetList(type As Type) As List(Of EnumItem)
        Dim arrayList As List(Of EnumItem) = New List(Of EnumItem)()
        Dim values = [Enum].GetValues(type)

        For Each v As [Enum] In values
            Dim item As EnumItem = New EnumItem
            item.Name = GetDescription(v)
            item.Value = Convert.ToInt32(v)
            arrayList.Add(item)
        Next

        Return arrayList
    End Function
End Class

Public Class EnumItem
    Public Property Name As String
    Public Property Value As Integer
    Public Property Description As String
End Class