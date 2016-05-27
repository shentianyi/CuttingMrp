Imports CuttingMrpLib
Imports Repository

Public Class RequirementRepository
    Inherits Repository(Of Requirement)
    Implements IRequirementRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = Me._dataContextFactory.Context
    End Sub



    Public Function Search(searchModel As RequirementSearchModel) As IQueryable(Of Requirement) Implements IRequirementRepository.Search
        If searchModel IsNot Nothing Then
            Dim requires As IQueryable(Of Requirement) = _context.Requirements
            requires.Where(Function(c) c.orderedDate >= searchModel.OrderedDateFrom)
            requires.Where(Function(c) c.orderedDate <= searchModel.OrderedDateTo)
            requires.Where(Function(c) c.requiredDate >= searchModel.RequiredTimeFrom)
            requires.Where(Function(c) c.requiredDate <= searchModel.RequiredTimeTo)
            If searchModel.QuantityFrom > 0 Then
                requires = requires.Where(Function(c) c.quantity >= searchModel.QuantityFrom)
            End If

            If searchModel.QuantityTo > 0 Then
                requires = requires.Where(Function(c) c.quantity <= searchModel.QuantityTo)
            End If

            If searchModel.Status <> -9999 Then
                requires = requires.Where(Function(c) c.status = searchModel.Status)
            End If

            If String.IsNullOrEmpty(searchModel.DerivedFrom) Then
                requires = requires.Where(Function(c) c.derivedFrom Like searchModel.DerivedFrom)
            End If

            If String.IsNullOrEmpty(searchModel.DerivedType) Then
                requires = requires.Where(Function(c) String.Compare(searchModel.DerivedType, c.derivedType))
            End If
            If String.IsNullOrEmpty(searchModel.PartNr) = True Then
                requires = requires.Where(Function(c) c.partNr Like searchModel.PartNr)
            End If
            requires = requires.Skip(searchModel.PageIndex * searchModel.PageSize)
            requires = requires.Take(searchModel.PageSize)
            Return requires

        Else
            Throw New ArgumentNullException
        End If
    End Function

    '    Private typs() As String = {"DERIVEDTYPE", "REQUIREDTIME_D", "REQUIREDTIME_W", "REQUIREDTIME_M", "ORDERTIME_D",
    ' "ORDERTIME_W", "ORDERTIME_M"}
    Public Function SearchStatistics(condition As RequirementStatisticsSearchModel) As IEnumerable(Of RequirementStatistics) Implements IRequirementRepository.SearchStatistics
        If condition IsNot Nothing Then

            Dim comm As String = "SELECT partNr AS PartNr,{0} AS Condition, SUM(quantity) AS SumOfQuantity FROM  dbo.Requirement GROUP BY partNr, {0}"
            Select Case condition.StatisticsType
                Case "DERIVEDTYPE"
                    comm = String.Format(comm, "derivedType", "derivedType")
                Case "REQUIREDTIME_D"
                    comm = String.Format(comm, "LEFT(CONVERT(nvarchar(30),requiredDate, 120), 10)")
                Case "REQUIREDTIME_W"
                    comm = String.Format(comm, "DATEPART(wk, requiredDate)")
                Case "REQUIREDTIME_M"
                    comm = String.Format(comm, "LEFT(CONVERT(nvarchar(30),requiredDate, 120), 7)")
                Case "ORDERTIME_D"
                    comm = String.Format(comm, "LEFT(CONVERT(nvarchar(30), orderedDate, 120), 10)")
                Case "ORDERTIME_W"
                    comm = String.Format(comm, "DATEPART(wk, orderedDate)")
                Case "ORDERTIME_M"
                    comm = String.Format(comm, "LEFT(CONVERT(nvarchar(30), orderedDate, 120), 7)")
                Case Else
                    Throw New Exception("type not supported")
            End Select
            Return _context.ExecuteQuery(Of RequirementStatistics)(comm, Nothing)
        Else
            Throw New ArgumentNullException
        End If
    End Function
End Class
