Imports CuttingMrpLib

Public Class BomItemService
    Inherits ServiceBase
    Implements IBomItemService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub
    Public Function Search(conditions As BomItemSearchModel) As IQueryable(Of BomItem) Implements IBomItemService.Search
        Dim reqRepo As IBomItemRepository = New BomItemRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function

    Public Function Create(bomItem As BomItem) As Boolean Implements IBomItemService.Create
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomItemRepository = New BomItemRepository(context)

        rep.MarkForAdd(bomItem)
        rep.SaveAll()
        result = True

        Return result
    End Function

    Public Function Update(bomItem As BomItem) As Boolean Implements IBomItemService.Update
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomItemRepository = New BomItemRepository(context)

        Dim ubomItem As BomItem = rep.FirstOrDefault(Function(s) s.id.Equals(bomItem.id))

        If (ubomItem IsNot Nothing) Then
            ubomItem.componentId = bomItem.componentId
            ubomItem.validFrom = bomItem.validFrom
            ubomItem.validTo = bomItem.validTo
            ubomItem.hasChind = bomItem.hasChind
            ubomItem.uom = bomItem.uom
            ubomItem.quantity = bomItem.quantity
            ubomItem.bomId = bomItem.bomId

            rep.SaveAll()

            result = True
        End If

        Return result
    End Function

    Public Function Delete(bomItem As BomItem) As Boolean Implements IBomItemService.Delete
        Dim result As Boolean = False
        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As BomItemRepository = New BomItemRepository(context)

        Dim ubomItem As BomItem = rep.FirstOrDefault(Function(s) s.id.Equals(bomItem.id))

        If (ubomItem IsNot Nothing) Then
            rep.MarkForDeletion(ubomItem)
            rep.SaveAll()

            result = True
        End If

        Return result
    End Function
End Class
