Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports CuttingMrpLib

<TestClass()> Public Class UnitTest1

    <TestMethod()> Public Sub TestListGroupBy()
        Dim li As List(Of Requirement) = New List(Of Requirement)
        li.Add(New Requirement With {.partNr = "P1", .requiredDate = New DateTime(2016, 1, 1, 12, 30, 50), .quantity = 10})
        li.Add(New Requirement With {.partNr = "P1", .requiredDate = New DateTime(2016, 1, 1, 12, 30, 50), .quantity = 20})
        li.Add(New Requirement With {.partNr = "P1", .requiredDate = New DateTime(2016, 1, 12, 12, 30, 50), .quantity = 30})
        li.Add(New Requirement With {.partNr = "P1", .requiredDate = New DateTime(2016, 1, 15, 12, 30, 50), .quantity = 40})
        li.Add(New Requirement With {.partNr = "P2", .requiredDate = New DateTime(2016, 1, 1, 12, 30, 50), .quantity = 50})
        li.Add(New Requirement With {.partNr = "P2", .requiredDate = New DateTime(2016, 1, 1, 12, 30, 50), .quantity = 60})
        li.Add(New Requirement With {.partNr = "P2", .requiredDate = New DateTime(2016, 1, 1, 12, 30, 50), .quantity = 70})

        '  Dim result As List(Of SumResult) = (From a In li.AsEnumerable Group By a.requiredDate Into total = Sum(a.quantity) Select a.partnr, a.requiredDate.ToString("YYYY-mm-DD"))
    End Sub

End Class

Public Class SumResult
    Private _partnr As String
    Private _date As DateTime
    Private _sum As Double

    Public Property Sum As Double
        Get
            Return _sum
        End Get
        Set(value As Double)
            _sum = value
        End Set
    End Property
    Public Property PartNr As String
        Get
            Return _partnr
        End Get
        Set(value As String)
            _partnr = value
        End Set
    End Property
    Public Property MyDate As DateTime
        Get
            Return _date
        End Get
        Set(value As DateTime)
            _date = value
        End Set
    End Property
End Class