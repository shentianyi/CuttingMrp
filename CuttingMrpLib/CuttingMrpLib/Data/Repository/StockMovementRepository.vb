﻿Imports CuttingMrpLib
Imports Repository

Public Class StockMovementRepository
    Inherits Repository.Repository(Of StockMovement)
    Implements IStockMovementRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub

    Public Function Search(conditions As StockMovementSearchModel) As IQueryable(Of StockMovement) Implements IStockMovementRepository.Search
        If conditions IsNot Nothing Then
            Dim moves As IQueryable(Of StockMovement) = _context.StockMovements
            If Not String.IsNullOrWhiteSpace(conditions.PartNr) Then
                moves = moves.Where(Function(m) m.partNr.Contains(conditions.PartNr.Trim()))
            End If

            If Not String.IsNullOrWhiteSpace(conditions.PartNrAct) Then
                moves = moves.Where(Function(m) m.partNr.Equals(conditions.PartNrAct.Trim()))
            End If

            If conditions.MoveType.HasValue Then
                moves = moves.Where(Function(c) c.moveType = conditions.MoveType)
            End If

            If conditions.DateFrom.HasValue Then
                moves = moves.Where(Function(m) m.createdAt >= conditions.DateFrom)
            End If


            If conditions.DateTo.HasValue Then
                moves = moves.Where(Function(m) m.createdAt <= conditions.DateTo)
            End If

            Return moves.OrderByDescending(Function(m) m.id)
        End If
        Return Nothing
    End Function
End Class