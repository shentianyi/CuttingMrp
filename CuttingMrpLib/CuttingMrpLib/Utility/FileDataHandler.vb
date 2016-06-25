Imports System.Globalization
Imports System.IO
Imports Brilliantech.Framwork.Utils.LogUtil

Public Class FileDataHandler
    ''' <summary>
    ''' fileName is full path of file to process
    ''' </summary>
    ''' <param name="fileName"></param>
    Public Sub ImportForceStock(fileName As String, db As String)
        If File.Exists(fileName) Then

            Dim ex As String = Path.GetExtension(fileName)
            Dim records As List(Of BatchFinishOrderRecord) = New List(Of BatchFinishOrderRecord)

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
                            Dim position As String = fields(4)
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
                            records.Add(record)
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
            Dim newDir As String = Path.Combine(dir, "Processed")
            If Directory.Exists(newDir) = False Then
                Directory.CreateDirectory(newDir)
            End If
            File.Move(fileName, Path.Combine(newDir, Path.GetFileName(fileName)))
        Else
            Throw New Exception(String.Format("处理Force文件失败,{0} 不存在", fileName))
        End If
    End Sub

End Class
