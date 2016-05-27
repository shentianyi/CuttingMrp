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
        serviceLogger.Info("MRP后台服务已经开始")
        timer = New System.Timers.Timer
        timer.Interval = My.Settings.interval
        timer.Enabled = True
        timer.Start()
    End Sub

    Protected Overrides Sub OnStop()
        serviceLogger.Warn("MRP后台服务已经结束")
    End Sub

    Private Sub timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles timer.Elapsed
        timer.Stop()
        Try
            Dim mrpExe As Calculator = New Calculator(My.Settings.db)
            If Not MessageQueue.Exists(My.Settings.queuepath) Then
                MessageQueue.Create(My.Settings.queuepath)
            End If
            Dim qu As MessageQueue = New MessageQueue(My.Settings.queuepath)
            qu.Formatter = New XmlMessageFormatter({GetType(String)})
            Dim msg As Message = qu.Receive
            If msg IsNot Nothing Then
                Dim settings As CalculateSetting = msg.Body
                mrpExe.ProcessMrp(settings)
            End If
        Catch ex As Exception
            businessLogger.Fatal("执行MRP程序时出错", ex)
        End Try
        timer.Start()
    End Sub
End Class
