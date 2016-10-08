Imports System.Globalization
Imports System.IO
Imports Brilliantech.Framwork.Utils.LogUtil
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports OfficeOpenXml

Public Class FileDataHandler
    ''' <summary>
    ''' fileName is full path of file to process
    ''' </summary>
    ''' <param name="fileName"></param>
    Public Sub ImportForceStock(fileName As String, db As String)
        Dim filterDate As DateTime = DateTime.ParseExact("2016-08-01", "yyyy-MM-dd", CultureInfo.CurrentCulture)
        If File.Exists(fileName) Then
            'Try
            Dim ex As String = Path.GetExtension(fileName)
            Dim records As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)

            Dim businessLogger As Object = Nothing

            If (ex.Equals(".csv")) Then
                Using treader As TextReader = File.OpenText(fileName)
                    For i As Integer = 0 To Integer.MaxValue
                        Dim s As String = treader.ReadLine

                        If i >= 16 Then
                            If String.IsNullOrWhiteSpace(s) Then
                                Exit For
                            End If

                            Dim fields As String() = s.Split(";")

                            Dim rdate As String = fields(0)
                            Dim time As String = fields(1)
                            Dim order As String = fields(2)
                            Dim position As String = fields(3)
                            Dim part As String = fields(8)
                            Dim kanban As String = fields(9)
                            Dim qty As String = fields(10)

                            Dim record As BatchFinishOrderRecord = New BatchFinishOrderRecord With {
                            .Id = String.Format("{0}_{1}_{2}", order, position, kanban),
                            .PartNr = part,
                            .FixOrderNr = kanban,
                            .Amount = Double.Parse(qty.Split(",")(0)),
                            .ProdTime = DateTime.ParseExact(rdate + " " + If(time.Length = 4, "0" + time, time), "dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)
                            }
                            If record.ProdTime > filterDate Then
                                records.Add(record)
                            End If
                        End If

                    Next
                End Using
            ElseIf (ex.Equals(".xlsx")) Then
                Dim fileInfo As FileInfo = New FileInfo(fileName)
                Using ep As ExcelPackage = New ExcelPackage(fileInfo)
                    Dim ws As ExcelWorksheet = ep.Workbook.Worksheets.First()
                    For i As Integer = 2 To ws.Dimension.End.Row
                        'Dim feedback = Integer.Parse(ws.Cells(i, 5).Value.ToString())
                        'If feedback > 0 Then
                        'Dim reasoncode As String = ws.Cells(i, 4).Value.ToString()
                        Dim time As String = ws.Cells(i, 4).Value.ToString()
                        Dim part As String = ws.Cells(i, 1).Value.ToString()
                            Dim packageid As String = ws.Cells(i, 2).Value.ToString()
                            Dim qty As String = ws.Cells(i, 3).Value.ToString()

                        Dim record As BatchFinishOrderRecord = New BatchFinishOrderRecord With {
                            .Id = packageid,
                            .PartNr = part,
                            .Amount = Double.Parse(qty.Split(",")(0)),
                            .ProdTime = time'DateTime.ParseExact(time, "dd.MM.yyyy", CultureInfo.CurrentCulture)
                            }

                        If record.ProdTime > filterDate Then
                                records.Add(record)
                            End If
                        'End If
                    Next
                End Using
            ElseIf (ex.Equals(".xls")) Then
                Using fs As FileStream = File.OpenRead(fileName)
                    Dim wk As HSSFWorkbook = New HSSFWorkbook(fs)
                    Dim sheet As ISheet = wk.GetSheetAt(0)
                    For i As Integer = 8 To sheet.LastRowNum
                        Dim feedback = Integer.Parse(sheet.GetRow(i).GetCell(16).NumericCellValue.ToString())
                        If feedback > 0 Then

                            If sheet.GetRow(i) IsNot Nothing Then
                                Dim rdate As String = sheet.GetRow(i).GetCell(0).StringCellValue
                                Dim time As String = sheet.GetRow(i).GetCell(1).StringCellValue
                                Dim part As String = sheet.GetRow(i).GetCell(10).StringCellValue
                                Dim kanban As String = sheet.GetRow(i).GetCell(12).NumericCellValue.ToString() ' sheet.GetRow(i).GetCell(12).StringCellValue
                                Dim qty As String = sheet.GetRow(i).GetCell(16).NumericCellValue.ToString() 'sheet.GetRow(i).GetCell(16).StringCellValue

                                Dim record As BatchFinishOrderRecord = New BatchFinishOrderRecord With {
                                .Id = String.Format("{0}_{1}_{2}", rdate, time, kanban),
                                .PartNr = part,
                                .FixOrderNr = kanban,
                                .Amount = Double.Parse(qty.Split(",")(0)),
                                .ProdTime = DateTime.ParseExact(rdate + " " + If(time.Length = 4, "0" + time, time), "dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)
                                }

                                If record.ProdTime > filterDate Then
                                    records.Add(record)
                                End If
                            End If
                        End If
                    Next
                End Using
            End If

            If records.Count > 0 Then
                Dim ps As IProcessOrderService = New ProcessOrderService(db)
                Dim results = ps.ValidateFinishOrder(records)
                ps.BatchFinishOrder(results("SUCCESS"), True, False)
            End If

            ' move file to processed file
            Dim dir As String = Path.GetDirectoryName(fileName)
            Dim newDir As String = Path.Combine(dir, "ProcessedOK")
            If Directory.Exists(newDir) = False Then
                Directory.CreateDirectory(newDir)
            End If
            File.Move(fileName, Path.Combine(newDir, Path.GetFileName(fileName)))
            'Catch ex As Exception
            '    Dim dir As String = Path.GetDirectoryName(fileName)
            '    Dim newDir As String = Path.Combine(dir, "ProcessedError")
            '    If Directory.Exists(newDir) = False Then
            '        Directory.CreateDirectory(newDir)
            '    End If
            '    File.Move(fileName, Path.Combine(newDir, Path.GetFileName(fileName)))
            '    Throw ex
            'End Try
        Else
            Throw New Exception(String.Format("处理Force文件失败,{0} 不存在", fileName))
        End If
    End Sub
End Class
