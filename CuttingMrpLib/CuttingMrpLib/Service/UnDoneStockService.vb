Imports CuttingMrpLib
Imports Repository

Public Class UnDoneStockService
    Inherits ServiceBase
    Implements IUnDoneStockService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Create(undonestock As UnDoneStock) As Boolean Implements IUnDoneStockService.Create
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As UnDoneStockRepository = New UnDoneStockRepository(context)

        If undonestock IsNot Nothing Then
            rep.MarkForAdd(undonestock)

            rep.SaveAll()
        End If

        Return Nothing
    End Function

    Public Function Search(conditions As UnDoneStockSearchModel) As IQueryable(Of UnDoneStock) Implements IUnDoneStockService.Search
        Dim reqRepo As IUnDoneStockRepository = New UnDoneStockRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function

    Public Function ValidateUnDoneStock(records As List(Of UnDoneStockRecord)) As Hashtable Implements IUnDoneStockService.ValidateUnDoneStock
        If records Is Nothing Then
            Throw New ArgumentNullException
        End If

        Dim warning As List(Of UnDoneStockRecord) = New List(Of UnDoneStockRecord)

        Dim succ As List(Of UnDoneStockRecord) = New List(Of UnDoneStockRecord)

        Dim kanbanRepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))

        For Each rec As UnDoneStockRecord In records
            Dim kanban As BatchOrderTemplate = kanbanRepo.FirstOrDefault(Function(c) c.orderNr.Equals(rec.PartNr))

            If kanban Is Nothing Then
                warning.Add(rec)
            Else
                rec.PartNr = kanban.partNr
                rec.SourceType = 1
                rec.State = 100
                succ.Add(rec)
            End If
        Next

        Dim result As Hashtable = New Hashtable

        If warning.Count > 0 Then
            result.Add("WARN", warning)
        End If

        If succ.Count > 0 Then
            result.Add("SUCCESS", succ)
        End If
        Return result

    End Function

    Public Function SetStateCancel() Implements IUnDoneStockService.SetStateCancel
        Dim context As DataContext = New DataContext(Me.DBConn)

        Dim rep As UnDoneStockRepository = New UnDoneStockRepository(context)

        Dim undonestocks As List(Of UnDoneStock) = rep.FindAll(Function(c) c.state = StockState.Finish And c.sourceType = PartType.BlueCard).ToList

        For Each undonestock As UnDoneStock In undonestocks
            undonestock.state = 200
        Next

        rep.SaveAll()

        Return Nothing

    End Function
End Class
