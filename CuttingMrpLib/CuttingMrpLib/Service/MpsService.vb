Imports CuttingMrpLib
Imports Repository

Public Class MpsService
    Inherits ServiceBase
    Implements IMpsService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub
    Public Function Create(mps As MP) As Boolean Implements IMpsService.Create
        Dim result As Boolean = False

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As MpsRepository = New MpsRepository(context)

        mps.orderedDate = Now
        mps.status = 1000
        mps.unitId = Nothing

        rep.MarkForAdd(mps)
        rep.SaveAll()

        result = True

        Return result
    End Function

    Public Function DeleteById(id As String) As Boolean Implements IMpsService.DeleteById
        Dim result As Boolean = False
        Dim repo As MpsRepository = New MpsRepository(New DataContext(DBConn))
        Dim mps As MP = repo.FirstOrDefault(Function(c) c.id = id)
        If mps Is Nothing Then
            Throw New Exception("Cannot find ID")
        Else
            repo.MarkForDeletion(mps)
            repo.SaveAll()
            result = True
        End If
        Return result
    End Function

    Public Function FindById(id As String) As MP Implements IMpsService.FindById
        Dim mp As MP = Nothing

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As MpsRepository = New MpsRepository(context)

        Return rep.FirstOrDefault(Function(o) o.id.ToString().Equals(id))
    End Function

    Public Function Search(searchModel As MpsSeachModel) As IQueryable(Of MP) Implements IMpsService.Search
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As IMpsRepository = New MpsRepository(context)

        Return rep.Search(searchModel)
    End Function

    Public Function Update(mps As MP) As Boolean Implements IMpsService.Update
        Dim result As Boolean = False

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As MpsRepository = New MpsRepository(context)
        Dim uId As MP = rep.FirstOrDefault(Function(s) s.id.Equals(mps.id))

        If (uId IsNot Nothing) Then
            uId.orderedDate = mps.orderedDate
            uId.requiredDate = mps.requiredDate
            uId.quantity = mps.quantity
            uId.source = mps.source
            uId.sourceDoc = mps.sourceDoc
            uId.status = mps.status
            rep.SaveAll()
            result = True
        End If
        Return result
    End Function
End Class
