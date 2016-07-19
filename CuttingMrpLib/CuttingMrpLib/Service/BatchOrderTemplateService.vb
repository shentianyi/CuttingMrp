Imports CuttingMrpLib

Public Class BatchOrderTemplateService
    Inherits ServiceBase
    Implements IBatchOrderTemplateService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub

    Public Function Search(conditions As BatchOrderTemplateSearchModel) As IQueryable(Of BatchOrderTemplate) Implements IBatchOrderTemplateService.Search
        Dim reqRepo As IBatchOrderTemplateRepository = New BatchOrderTemplateRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function

    Public Function Create(batchOrderTempalte As BatchOrderTemplate) As Boolean Implements IBatchOrderTemplateService.Create
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BatchOrderTemplateRepository = New BatchOrderTemplateRepository(context)

        rep.MarkForAdd(batchOrderTempalte)
        rep.SaveAll()
        result = True

        Return result
    End Function

    Public Function Delete(batchOrderTempalte As BatchOrderTemplate) As Boolean Implements IBatchOrderTemplateService.Delete
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BatchOrderTemplateRepository = New BatchOrderTemplateRepository(context)

        Dim ubatchOrderTemplate As BatchOrderTemplate = rep.FirstOrDefault(Function(s) s.orderNr.Equals(batchOrderTempalte.orderNr))

        If (ubatchOrderTemplate IsNot Nothing) Then
            rep.MarkForDeletion(ubatchOrderTemplate)
            rep.SaveAll()

            result = True
        End If

        Return result

    End Function

    Public Function Update(batchOrderTempalte As BatchOrderTemplate) As Boolean Implements IBatchOrderTemplateService.Update
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BatchOrderTemplateRepository = New BatchOrderTemplateRepository(context)

        Dim ubatchOrderTemplate As BatchOrderTemplate = rep.FirstOrDefault(Function(s) s.orderNr.Equals(batchOrderTempalte.orderNr))

        If (ubatchOrderTemplate IsNot Nothing) Then
            ubatchOrderTemplate.partNr = batchOrderTempalte.partNr
            ubatchOrderTemplate.batchQuantity = batchOrderTempalte.batchQuantity
            ubatchOrderTemplate.type = batchOrderTempalte.type
            ubatchOrderTemplate.bundle = batchOrderTempalte.bundle
            ubatchOrderTemplate.createdAt = batchOrderTempalte.createdAt
            ubatchOrderTemplate.updatedAt = batchOrderTempalte.updatedAt
            ubatchOrderTemplate.operator = batchOrderTempalte.operator
            ubatchOrderTemplate.remark1 = batchOrderTempalte.remark1
            ubatchOrderTemplate.remark2 = batchOrderTempalte.remark2

            rep.SaveAll()

            result = True
        End If

        Return result
    End Function

    Public Function FindByNr(nr As String) As BatchOrderTemplate Implements IBatchOrderTemplateService.FindByNr
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BatchOrderTemplateRepository = New BatchOrderTemplateRepository(context)
        Return rep.FirstOrDefault(Function(s) s.orderNr.Equals(nr))

    End Function
End Class
