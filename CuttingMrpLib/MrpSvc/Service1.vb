Imports System.Timers
Imports System.Messaging
Imports System.Text.RegularExpressions
Imports log4net
Imports System.IO
Imports CuttingMrpLib


Public Class Service1
    Private Shared serviceLogger As ILog = LogManager.GetLogger("ServiceLog")
    Private Shared businessLogger As ILog = LogManager.GetLogger("BizLog")
    Private WithEvents timer As System.Timers.Timer
    Protected Overrides Sub OnStart(ByVal args() As String)
        log4net.Config.XmlConfigurator.ConfigureAndWatch(New FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logConfig.xml")))
        serviceLogger.Warn("MRP后台服务已经开始")
        timer = New System.Timers.Timer
        timer.Interval = My.Settings.interval
        timer.Enabled = True
        timer.Start()
    End Sub

    Protected Overrides Sub OnStop()
        serviceLogger.Fatal("MRP后台服务已经结束,此服务用于计算Cutting的MRP，如果服务没有开启，则无法进行计算")
    End Sub

    Private Sub timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles timer.Elapsed
        timer.Stop()
        Try
            Dim mrpExe As Calculator = New Calculator(My.Settings.db)
            If Not MessageQueue.Exists(My.Settings.queuepath) Then
                MessageQueue.Create(My.Settings.queuepath)
            End If
            Dim qu As MessageQueue = New MessageQueue(My.Settings.queuepath)
            qu.Formatter = New XmlMessageFormatter({GetType(CalculateSetting)})
            Dim msg As Message = qu.Receive
            If msg IsNot Nothing Then
                Dim settings As CalculateSetting = msg.Body
                If settings.TaskType = "MRP" Then
                    mrpExe.ProcessMrp(settings)
                ElseIf settings.TaskType = "BF" Then
                    mrpExe.MakeBackflush()
                Else
                    Throw New Exception("Unsupported Task Type " & settings.TaskType)
                End If

            End If
            businessLogger.Warn("MRP运行结束，您可以即刻查看运算出的需求")
        Catch ex As Exception
            businessLogger.Fatal("执行MRP程序时出错", ex)
        End Try
        timer.Start()
    End Sub
End Class
