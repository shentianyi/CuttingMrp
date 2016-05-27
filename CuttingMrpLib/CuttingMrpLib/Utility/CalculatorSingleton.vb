Imports System.Messaging
Imports Repository

Public Class CalculatorSingleton
    Private Shared instance As CalculatorSingleton

    Private Sub New()

    End Sub

    Private Shared threadLocker As New Object
    Public Shared Function CreateInstance() As CalculatorSingleton
        If instance Is Nothing Then
            SyncLock threadLocker
                If instance Is Nothing Then
                    instance = New CalculatorSingleton
                End If
            End SyncLock
        End If
        Return instance
    End Function

    Private Shared exelocker As New Object
    Public Sub DoCalculation(type As String, db As String, queueAddr As String, settings As CalculateSetting)
        SyncLock exelocker
            If Not MessageQueue.Exists(queueAddr) Then
                MessageQueue.Create(queueAddr, False)
            End If
            Dim qu As MessageQueue = New MessageQueue(queueAddr)
            Select Case type
                Case "START"
                    If qu.GetAllMessages.Count > 0 Then
                        Throw New Exception("队列中已经有待运行的任务，请稍后再试")
                    End If
                    Dim mrpRepo As Repository(Of MrpRound) = New Repository(Of MrpRound)(New DataContext(db))
                    Dim mrp As MrpRound = mrpRepo.First(Function(c) c.runningStatus = CalculatorStatus.Running)
                    If mrp IsNot Nothing Then
                        Throw New Exception("MRP is running")
                    End If
                    Dim msg As Message = New Message

                    msg.Body = settings
                    msg.Formatter = New XmlMessageFormatter({GetType(String)})
                    qu.Send(msg)
                Case Else
                    Throw New Exception("MRP Action not supported")
            End Select

        End SyncLock
    End Sub

End Class
