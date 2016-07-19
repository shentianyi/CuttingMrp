Imports System.Transactions
Imports CuttingMrpLib
Imports Repository


Public Class ProcessOrderService
    Inherits ServiceBase
    Implements IProcessOrderService

    Public Sub New(db As String)
        MyBase.New(db)
    End Sub



    Public Function FindOrderTemplateByPartNr(partNr As String) As IEnumerable(Of BatchOrderTemplate) Implements IProcessOrderService.FindOrderTemplateByPartNr
        Dim fixOrderRepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))
        Return fixOrderRepo.FindAll(Function(c) c.partNr = partNr)
    End Function



    Public Sub CancelOrdersByIds(ids As List(Of String), isSystem As Boolean) Implements IProcessOrderService.CancelOrdersByIds
        If ids Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
            Dim toCancel As IEnumerable(Of ProcessOrder) = repo.FindAll(Function(c) c.status = ProcessOrderStatus.Open And ids.Contains(c.orderNr))
            For Each toCancelOrder As ProcessOrder In toCancel
                If isSystem = True Then
                    toCancelOrder.status = ProcessOrderStatus.SystemCancel
                Else
                    toCancelOrder.status = ProcessOrderStatus.ManualCancel
                End If
            Next
            repo.SaveAll()
        End If
    End Sub

    Public Sub UpdateOrderQuantity(toUpdateId As String, quantity As Double) Implements IProcessOrderService.UpdateOrderQuantity
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Dim toupdate As ProcessOrder = repo.FirstOrDefault(Function(c) c.orderNr = toUpdateId)
        If toupdate Is Nothing Then
            Throw New Exception("Cannot find Order")
        Else
            toupdate.actualQuantity = quantity
        End If
        repo.SaveAll()
    End Sub

    Public Function Search(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrder) Implements IProcessOrderService.Search

        Dim reqRepo As IProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))

        Return reqRepo.Search(conditions)
    End Function

    Public Function DeleteById(id As String) As Boolean Implements IProcessOrderService.DeleteById
        Dim result As Boolean = False
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Dim order As ProcessOrder = repo.FirstOrDefault(Function(c) c.orderNr = id)
        If order Is Nothing Then
            Throw New Exception("Cannot find Order")
        Else
            If order.canDelete Then
                repo.MarkForDeletion(order)
                repo.SaveAll()
                result = True
            Else
                Throw New Exception("Order cannot deleted!")
            End If
        End If
        Return result
    End Function

    Public Function FindById(id As String) As ProcessOrder Implements IProcessOrderService.FindById
        Dim processOrder As ProcessOrder = Nothing

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As ProcessOrderRepository = New ProcessOrderRepository(context)
        Return rep.FirstOrDefault(Function(o) o.orderNr.Equals(id))
    End Function

    Public Function Update(processOrder As ProcessOrder) As Boolean Implements IProcessOrderService.Update
        Dim result As Boolean = False

        Dim context As DataContext = New DataContext(Me.DBConn)
        Dim rep As ProcessOrderRepository = New ProcessOrderRepository(context)
        Dim uorder As ProcessOrder = rep.FirstOrDefault(Function(s) s.orderNr.Equals(processOrder.orderNr))
        If (uorder IsNot Nothing) Then
            'uorder.proceeDate = processOrder.proceeDate

            uorder.actualQuantity = processOrder.actualQuantity
            uorder.status = processOrder.status

            rep.SaveAll()
            result = True
        End If
        Return result
    End Function

    Public Sub FinishOrdersByIds(ids As List(Of String),
                                 fifo As DateTime,
                                 container As String,
                                 wh As String, position As String,
                                 source As String, sourceType As String, moveType As StockMoveType, Optional enterStock As Boolean = True) Implements IProcessOrderService.FinishOrdersByIds
        If ids Is Nothing Then
            Throw New ArgumentNullException
        Else
            Dim context = New DataContext(DBConn)
            Dim repo As ProcessOrderRepository = New ProcessOrderRepository(context)


            Dim toFinish As List(Of ProcessOrder) = repo.FindAll(Function(c) c.status = ProcessOrderStatus.Open And ids.Contains(c.orderNr)).ToList

            Using scope As New TransactionScope
                ' finish 
                For Each toFinishOrder As ProcessOrder In toFinish
                    toFinishOrder.status = ProcessOrderStatus.Finish
                Next
                If enterStock Then
                    Dim stockRep As StockRepository = New StockRepository(context)
                    Dim moveRep As Repository(Of StockMovement) = New Repository(Of StockMovement)(context)
                    Dim moves As List(Of StockMovement) = New List(Of StockMovement)
                    Dim stocks As List(Of Stock) = New List(Of Stock)
                    For Each toFinishOrder As ProcessOrder In toFinish
                        stocks.Add(New Stock With {.partNr = toFinishOrder.partNr,
                               .fifo = fifo,
                               .quantity = toFinishOrder.actualQuantity,
                               .container = container,
                               .wh = wh,
                               .position = position,
                               .source = toFinishOrder.orderNr,
                               .sourceType = "ProcessOrder"})
                        moves.Add(New StockMovement With {.fifo = fifo,
                                  .moveType = moveType, .partNr = toFinishOrder.partNr,
                                  .quantity = toFinishOrder.actualQuantity,
                                  .sourceDoc = toFinishOrder.orderNr,
                                  .createdAt = DateTime.Now})
                    Next
                    If stocks.Count > 0 Then
                        stockRep.GetTable.InsertAllOnSubmit(stocks)
                        stockRep.SaveAll()
                        moveRep.GetTable.InsertAllOnSubmit(moves)
                        moveRep.SaveAll()
                    End If
                End If
                repo.SaveAll()
                scope.Complete()
            End Using
        End If
    End Sub

    Public Function GetProcessOrderInfo(conditions As ProcessOrderSearchModel) As ProcessOrderInfoModel Implements IProcessOrderService.GetProcessOrderInfo

        Dim info As ProcessOrderInfoModel = New ProcessOrderInfoModel
        Dim context = New DataContext(DBConn)
        Dim repo As ProcessOrderRepository = New ProcessOrderRepository(context)
        Dim q As IQueryable(Of ProcessOrder) = repo.Search(conditions)


        info.latestOrder = q.OrderByDescending(Function(c) c.proceeDate).FirstOrDefault()
        info.oldestOrder = q.OrderBy(Function(c) c.proceeDate).FirstOrDefault()
        info.processOrderCount = q.Count
        info.requirementCount = context.Context.GetTable(Of OrderDerivation).Where(Function(o) q.Select(Function(c) c.orderNr).Contains(o.orderId)).Count
        'q.Select(Function(c) c.OrderDerivations).Distinct.Count


        Return info

    End Function



    Public Sub BatchFinishOrder(records As List(Of BatchFinishOrderRecord), ignoreError As Boolean, Optional reVali As Boolean = True) Implements IProcessOrderService.BatchFinishOrder
        Dim recs As List(Of BatchFinishOrderRecord) = records
        If reVali Then
            Dim validated As Hashtable = ValidateFinishOrder(records)
            recs = validated("SUCCESS")
            If validated.ContainsKey("WARN") Then
                If ignoreError = False Then
                    Throw New Exception("文件验证失败")
                End If
            End If
        End If

        Dim conditions As ProcessOrderSearchModel = New ProcessOrderSearchModel With {.Status = ProcessOrderStatus.Open, .PageSize = Integer.MaxValue}

        Dim toStock As List(Of Stock) = New List(Of Stock)
        Dim negaStockDic As Dictionary(Of String, List(Of Stock)) = New Dictionary(Of String, List(Of Stock))

        Dim posiStockDic As Dictionary(Of String, List(Of Stock)) = New Dictionary(Of String, List(Of Stock))
        Dim toMove As List(Of StockMovement) = New List(Of StockMovement)
        Dim toCreateStockBatchMoveRecord As List(Of StockBatchMoveRecord) = New List(Of StockBatchMoveRecord)

        Dim findContext As DataContext = New DataContext(DBConn)
        Dim findStockRep As StockRepository = New StockRepository(findContext)
        Dim findStockBatchRecordRep As Repository(Of StockBatchMoveRecord) = New Repository(Of StockBatchMoveRecord)(findContext)


        For Each rec As BatchFinishOrderRecord In recs
            If findStockBatchRecordRep.FirstOrDefault(Function(s) s.id.Equals(rec.Id)) Is Nothing Then

                Dim tmpQuantity As Double = rec.Amount

                If tmpQuantity < 0 Then
                    If posiStockDic.ContainsKey(rec.PartNr) = False Then
                        Dim posiStocks As List(Of Stock) = findStockRep.FindAll(Function(s) s.partNr.Equals(rec.PartNr) And s.quantity > 0).OrderBy(Function(s) s.fifo).ToList
                        If posiStocks.Count > 0 Then
                            posiStockDic.Add(rec.PartNr, posiStocks)
                        End If
                    End If

                    If posiStockDic.ContainsKey(rec.PartNr) Then
                        For Each s As Stock In posiStockDic(rec.PartNr)
                            If rec.Amount < 0 And s.quantity > 0 Then
                                If Math.Abs(rec.Amount) > s.quantity Then
                                    rec.Amount += s.quantity
                                    s.quantity = 0
                                Else
                                    s.quantity += rec.Amount
                                    rec.Amount = 0
                                End If
                            End If
                        Next
                    End If
                End If

                ' 找到负库存
                If negaStockDic.ContainsKey(rec.PartNr) = False Then
                    Dim negaStocks As List(Of Stock) = findStockRep.FindAll(Function(s) s.partNr.Equals(rec.PartNr) And s.quantity <= 0).ToList
                    If negaStocks.Count > 0 Then
                        negaStockDic.Add(rec.PartNr, negaStocks)
                    End If
                End If

                If negaStockDic.ContainsKey(rec.PartNr) Then
                    For Each s As Stock In negaStockDic(rec.PartNr)
                        If rec.Amount <> 0 And s.quantity <= 0 Then
                            If Math.Abs(s.quantity) >= rec.Amount Then

                                s.quantity += rec.Amount ' quantity 逐渐变大 
                                rec.Amount = 0
                            Else
                                rec.Amount += s.quantity 'amount 逐渐变小
                                s.quantity = 0
                            End If
                        End If
                    Next
                End If

                If rec.Amount <> 0 Then
                    toStock.Add(New Stock With {.partNr = rec.PartNr, .fifo = rec.ProdTime,
                                       .sourceType = "BATCHUPLOAD", .container = "ORIGINAL",
                                   .position = "ORIGINAL", .quantity = rec.Amount,
                                    .source = "BATCHUPLOAD", .wh = "ORGINAL"})
                End If



                toMove.Add(New StockMovement With {.fifo = Now,
                                  .moveType = rec.MoveType, .partNr = rec.PartNr,
                                  .quantity = tmpQuantity,
                                  .sourceDoc = rec.FixOrderNr,
                                  .createdAt = DateTime.Now})

                toCreateStockBatchMoveRecord.Add(New StockBatchMoveRecord With {.id = rec.Id,
                                                 .moveType = rec.MoveType,
                                                 .partNr = rec.PartNr,
                                                 .quantity = tmpQuantity,
                                                 .sourceDoc = rec.FixOrderNr,
                                                 .souceDocTime = rec.ProdTime,
                                                 .createdAt = DateTime.Now})
            End If
        Next

        Using scope As New TransactionScope(TransactionScopeOption.Required, New TransactionOptions() With {.Timeout = New TimeSpan(0, 30, 0)})


            Dim context As DataContext = New DataContext(DBConn)
            Dim stockrepo As Repository(Of Stock) = New Repository(Of Stock)(context)
            Dim moveRep As Repository(Of StockMovement) = New Repository(Of StockMovement)(context)
            Dim batchRecordRep As Repository(Of StockBatchMoveRecord) = New Repository(Of StockBatchMoveRecord)(context)


            ' Dim deleteStock As List(Of Stock) = New List(Of Stock)
            For Each stocks As List(Of Stock) In negaStockDic.Values
                Dim deleteIds As List(Of Integer) = stocks.Where(Function(s) s.quantity.Equals(0)).Distinct.Select(Function(s) s.id).ToList
                If deleteIds.Count > 0 Then
                    Dim _deletes As List(Of Stock) = context.Context.GetTable(Of Stock).Where(Function(s) deleteIds.Contains(s.id)).ToList
                    context.Context.GetTable(Of Stock).DeleteAllOnSubmit(_deletes)
                End If

                Dim updateIds As List(Of Integer) = stocks.Where(Function(s) s.quantity < 0).Distinct.Select(Function(s) s.id).ToList
                If updateIds.Count > 0 Then
                    Dim _updates As List(Of Stock) = context.Context.GetTable(Of Stock).Where(Function(s) updateIds.Contains(s.id)).ToList
                    For Each u As Stock In _updates
                        Dim ori As Stock = stocks.SingleOrDefault(Function(s) s.id.Equals(u.id))
                        If ori IsNot Nothing Then
                            u.quantity = ori.quantity
                        End If
                    Next
                End If
                'For Each stock As Stock In stocks.Where(Function(s) s.quantity.Equals(0)).ToList
                '    Dim s As Stock = stockrepo.FirstOrDefault(Function(ss) ss.id.Equals(stock.id))
                '    If s IsNot Nothing Then
                '        stockrepo.(s)
                '    End If
                'Next
                'Dim _updateStock As List(Of Stock) = stocks.Where(Function(s) s.quantity < 0).Distinct.ToList

                'For Each stock As Stock In _updateStock
                '    Dim s As Stock = stockrepo.FirstOrDefault(Function(ss) ss.id.Equals(stock.id))
                '    If s IsNot Nothing Then
                '        s.quantity = stock.quantity
                '    End If
                'Next
            Next



            ' Dim deleteStock As List(Of Stock) = New List(Of Stock)
            For Each stocks As List(Of Stock) In posiStockDic.Values
                Dim deleteIds As List(Of Integer) = stocks.Where(Function(s) s.quantity.Equals(0)).Distinct.Select(Function(s) s.id).ToList
                If deleteIds.Count > 0 Then
                    Dim _deletes As List(Of Stock) = context.Context.GetTable(Of Stock).Where(Function(s) deleteIds.Contains(s.id)).ToList
                    context.Context.GetTable(Of Stock).DeleteAllOnSubmit(_deletes)
                End If

                Dim updateIds As List(Of Integer) = stocks.Where(Function(s) s.quantity > 0).Distinct.Select(Function(s) s.id).ToList
                If updateIds.Count > 0 Then
                    Dim _updates As List(Of Stock) = context.Context.GetTable(Of Stock).Where(Function(s) updateIds.Contains(s.id)).ToList
                    For Each u As Stock In _updates
                        Dim ori As Stock = stocks.SingleOrDefault(Function(s) s.id.Equals(u.id))
                        If ori IsNot Nothing Then
                            u.quantity = ori.quantity
                        End If
                    Next
                End If
            Next
            context.Context.GetTable(Of Stock).InsertAllOnSubmit(toStock)
            context.Context.GetTable(Of StockMovement).InsertAllOnSubmit(toMove)
            context.Context.GetTable(Of StockBatchMoveRecord).InsertAllOnSubmit(toCreateStockBatchMoveRecord)

            context.SaveAll()
            scope.Complete()
            ' Catch ex As Exception
            '  Throw New Exception("写入数据库时出现错误", ex)
            ' End Try
        End Using


    End Sub

    Private Function PartExists(partNr As String) As Boolean
        Dim partRepo As Repository(Of Part) = New Repository(Of Part)(New DataContext(DBConn))
        Dim counter As Integer = partRepo.Count(Function(c) c.partNr = partNr)
        If counter > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ValidateFinishOrder(records As List(Of BatchFinishOrderRecord)) As Hashtable Implements IProcessOrderService.ValidateFinishOrder
        If records Is Nothing Then
            Throw New ArgumentNullException
        End If
        Dim warning As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)
        Dim succ As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)

        Dim kanbanRepo As Repository(Of BatchOrderTemplate) = New Repository(Of BatchOrderTemplate)(New DataContext(DBConn))

        For Each rec As BatchFinishOrderRecord In records
            Dim kanban As BatchOrderTemplate = kanbanRepo.FirstOrDefault(Function(c) c.orderNr = rec.FixOrderNr)
            If kanban Is Nothing Then
                rec.Warnings.Add("Kanban: " & rec.FixOrderNr & " does not exist in the system")
                warning.Add(rec)
            Else
                rec.PartNr = kanban.partNr
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

    Public Function SearchView(conditions As ProcessOrderSearchModel) As IQueryable(Of ProcessOrderView) Implements IProcessOrderService.SearchView
        Dim reqRepo As IProcessOrderRepository = New ProcessOrderRepository(New DataContext(DBConn))
        Return reqRepo.SearchView(conditions)
    End Function
End Class
