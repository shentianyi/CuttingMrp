Imports CuttingMrpLib

Public Class BomService
    Inherits ServiceBase
    Implements IBomService
    Public Sub New(db As String)
        MyBase.New(db)
    End Sub
    Public Function Search(conditions As BomSearchModel) As IQueryable(Of BOM) Implements IBomService.Search
        Dim reqRepo As IBomRepository = New BomRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function


    Public Function Create(bom As BOM) As Boolean Implements IBomService.Create
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomRepository = New BomRepository(context)

        rep.MarkForAdd(bom)
        rep.SaveAll()
        result = True

        Return result
    End Function

    Public Function Delete(bom As BOM) As Boolean Implements IBomService.Delete
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomRepository = New BomRepository(context)

        Dim ubom As BOM = rep.FirstOrDefault(Function(s) s.id.Equals(bom.id))

        If (ubom IsNot Nothing) Then
            rep.MarkForDeletion(bom)
            rep.SaveAll()

            result = True
        End If

        Return result
    End Function

    Public Function Update(bom As BOM) As Boolean Implements IBomService.Update
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomRepository = New BomRepository(context)

        Dim ubom As BOM = rep.FirstOrDefault(Function(s) s.id.Equals(bom.id))

        If (ubom IsNot Nothing) Then
            ubom.partNr = bom.partNr
            ubom.validFrom = bom.validFrom
            ubom.validTo = bom.validTo
            ubom.versionId = bom.versionId
            ubom.bomDesc = bom.bomDesc

            rep.SaveAll()

            result = True
        End If

        Return result
    End Function
End Class
