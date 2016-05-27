Imports System.Transactions
Imports Repository
Public Class NumericDefinitionRepository
    Inherits Repository(Of NumericBuild)
    Implements INumericBuildRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(ByVal dc As Repository.IDataContextFactory)
        MyBase.New(dc)
        _context = dc.Context
    End Sub



    Private locker As New Object
    Private Function GenerateID(ByVal numericType As String) As String
        SyncLock locker
            Dim numID As String = ""
            Dim innerex As Exception = New Exception
            Using scope As New TransactionScope()
                Try

                    Dim num As NumericBuild

                    num = (From nums In _context.NumericBuilds Where nums.idType = numericType Select nums).SingleOrDefault

                    If num Is Nothing Then
                        Throw New Exception("没有找到类型为" & numericType & " 的ID预设")
                    End If



                    If num.currentNumber + 1 >= num.max Then
                        Throw New Exception("id号已经到达预设最大值")
                    End If

                    num.currentNumber = num.currentNumber + 1
                    _context.SubmitChanges()
                    scope.Complete()
                    numID = num.prefix & (num.currentNumber).ToString("D" & num.max.ToString.Length.ToString) & num.suffix
                Catch ex As Exception
                    innerex = ex
                End Try
            End Using
            If String.IsNullOrEmpty(numID) Then
                Throw New Exception("没有获取到id号", innerex)
            End If

            Return numID
        End SyncLock
    End Function


    Public Function Generate(ByVal numericType As String) As String Implements INumericBuildRepository.Generate
        Return GenerateID(numericType)
        'Dim numID As String = ""
        'Dim innerex As Exception = New Exception
        'Using scope As New TransactionScope()
        '    Try

        '        Dim num As numeric
        '        Dim _db As SVWDataFactoryDataContext = New SVWDataFactoryDataContext(connection)
        '        num = (From nums In _db.numerics Where nums.numericID = numericType Select nums).SingleOrDefault
        '        num.currentNumeric = num.currentNumeric + 1
        '        _db.SubmitChanges()
        '        scope.Complete()
        '        numID = num.prefix & num.seperator & (num.currentNumeric).ToString("D" & num.max.ToString.Length.ToString)
        '    Catch ex As Exception
        '        innerex = ex
        '    End Try
        'End Using
        'If String.IsNullOrEmpty(numID) Then
        '    Throw New Exception("没有获取到id号", innerex)
        'End If

        'Return numID
    End Function

    'Public Sub Save()
    '    db.SubmitChanges()
    'End Sub
End Class
