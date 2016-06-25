Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports CuttingMrpLib
Imports System.Messaging
<TestClass()> Public Class UnitTest1

    <TestMethod()> Public Sub TestBackflush()
        Dim processor As Calculator = New Calculator("Data Source=WANGSONG-PC\MSSQLSERVER2008R;Initial Catalog=CuttingMrp;Persist Security Info=True;User ID=sa;Password=123456@")
        Try
            processor.MakeBackflush()
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub


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


    <TestMethod> Public Sub TestConvertMpsToRequirement()
        Dim cal As Calculator = New Calculator("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
        Try
            '  cal.ConvertMpsToRequirement()
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    <TestMethod> Public Sub TestGenerate()
        Dim cal As Calculator = New Calculator("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
        Try
            cal.GenerateProcessOrderByRequirement("0003", New CalculateSetting With {.MergeMethod = New MergeMethod With {.Count = 2, .FirstDay = New DateTime(2016, 6, 1), .MergeType = "DAY"}, .OrderType = "FIX", .RoundId = "001"})

        Catch ex As Exception

        End Try
    End Sub

    <TestMethod> Public Sub TestProcessOrderBatchFinish()
        Dim cal As ProcessOrderService = New ProcessOrderService("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")

        Dim ls As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)
        ls.Add(New BatchFinishOrderRecord With {.FixOrderNr = "410714", .Amount = 250, .LineNr = 1, .ProdTime = Now})
        ls.Add(New BatchFinishOrderRecord With {.FixOrderNr = "3112189", .Amount = 250, .LineNr = 1, .ProdTime = Now})
        ls.Add(New BatchFinishOrderRecord With {.FixOrderNr = "3112449", .Amount = 20, .LineNr = 1, .ProdTime = Now})

        Dim hs As Hashtable = cal.ValidateFinishOrder(ls)
        Assert.IsTrue(hs("WARN").count = 2)
        Assert.IsTrue(hs("SUCCESS").count = 1)

        Try
            cal.BatchFinishOrder(ls, False)
            Assert.Fail()
        Catch ex As Exception

        End Try

        cal.BatchFinishOrder(ls, True)

    End Sub

    '<TestMethod> Public Sub TestQueue()
    '    Dim cal As CalculateService = New CalculateService("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
    '    Try
    '        cal.Start(".\Private$\CuttingMrp", New CalculateSetting With {.MergeMethod = "DAY", .OrderType = "FIX"})
    '    Catch ex As Exception

    '    End Try
    'End Sub

    <TestMethod> Public Sub TestQueueRead()
        Dim cal As Calculator = New Calculator("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
        Try
            Dim mrpExe As Calculator = New Calculator("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
            If Not MessageQueue.Exists(".\Private$\CuttingMrp") Then
                MessageQueue.Create(".\Private$\CuttingMrp")
            End If
            Dim qu As MessageQueue = New MessageQueue(".\Private$\CuttingMrp")
            qu.Formatter = New XmlMessageFormatter({GetType(CalculateSetting)})
            Dim msg As Message = qu.Receive
            If msg IsNot Nothing Then
                Dim settings As CalculateSetting = msg.Body
                mrpExe.ProcessMrp(settings)
            End If
        Catch ex As Exception

        End Try
    End Sub


    <TestMethod> Public Sub TestFindDate()
        Dim cal As Calculator = New Calculator("Data Source=vm08;Initial Catalog=CuttingMrp;User ID=sa;Password=brilliantech123@")
        Dim udays As Integer = (New DateTime(2016, 5, 31) - New DateTime(2016, 6, 5)).Days
        Dim datetime As DateTime = cal.FindBasicDate(Now, Now, 2)
        Assert.IsNotNull(datetime)
        datetime = cal.FindBasicDate(Now, New DateTime(2016, 6, 5), 2)
        Assert.IsNotNull(datetime)
        datetime = cal.FindBasicDate(Now, New DateTime(2016, 5, 26), 2)
        Assert.IsNotNull(datetime)
        datetime = cal.FindBasicDate(New DateTime(2016, 6, 1), Now, 2)
        Assert.IsNotNull(datetime)
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