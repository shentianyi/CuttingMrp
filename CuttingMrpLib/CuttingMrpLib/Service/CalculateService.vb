Imports CuttingMrpLib
Imports System.Messaging

Public Class CalculateService
    Inherits ServiceBase
    Implements ICalculateService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub
    ''' <summary>
    ''' 1. must no message in the queue
    ''' 2. must no mrp round marked as running
    ''' </summary>
    ''' <param name="queueAddr"></param>
    ''' <param name="Settings"></param>
    Public Sub Start(queueAddr As String, settings As CalculateSetting) Implements ICalculateService.Start
        If settings Is Nothing Then
            Throw New ArgumentNullException
        Else
            CalculatorSingleton.CreateInstance.DoCalculation("START", DBConn, queueAddr, settings)
        End If
    End Sub

    Public Function Search(conditons As MRPSearchModel) As IQueryable(Of MrpRound) Implements ICalculateService.Search
        Dim calculateRepo As CalculateRepository = New CalculateRepository(New DataContext(DBConn))
        Return calculateRepo.Search(conditons)
    End Function
End Class
