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
            If searchModel.OrderedDateFrom.HasValue Then
                requires = requires.Where(Function(c) c.orderedDate >= searchModel.OrderedDateFrom)
            End If
            If searchModel.OrderedDateTo.HasValue Then
                requires = requires.Where(Function(c) c.orderedDate <= searchModel.OrderedDateTo)
            End If
            If searchModel.RequiredTimeFrom.HasValue Then
                requires = requires.Where(Function(c) c.requiredDate >= searchModel.RequiredTimeFrom)
            End If
            If searchModel.RequiredTimeTo.HasValue Then
                requires = requires.Where(Function(c) c.requiredDate <= searchModel.RequiredTimeTo)
            End If

            If searchModel.QuantityFrom.HasValue > 0 Then
                requires = requires.Where(Function(c) c.quantity >= searchModel.QuantityFrom)
            End If

            If searchModel.QuantityTo.HasValue > 0 Then
                requires = requires.Where(Function(c) c.quantity <= searchModel.QuantityTo)
            End If

            If searchModel.Status.HasValue Then
                requires = requires.Where(Function(c) c.status = searchModel.Status)
            End If

            If Not String.IsNullOrEmpty(searchModel.DerivedFrom) Then
                requires = requires.Where(Function(c) c.derivedFrom.Contains(searchModel.DerivedFrom))
            End If

            If Not String.IsNullOrEmpty(searchModel.DerivedType) Then
                requires = requires.Where(Function(c) String.Compare(searchModel.DerivedType, c.derivedType))
            End If

            If Not String.IsNullOrEmpty(searchModel.PartNr) Then
                requires = requires.Where(Function(c) c.partNr.Contains(searchModel.PartNr))
            End If

            If Not String.IsNullOrWhiteSpace(searchModel.OrderNr) Then
                Dim ids = _context.OrderDerivations.Where(Function(cc) cc.orderId.Equals(searchModel.OrderNr)).Select(Function(cc) cc.requirementId).ToList
                requires = requires.Where(Function(c) ids.Contains(c.id))
            End If


            '   requires = requires.Skip(searchModel.PageIndex * searchModel.PageSize)
            '  requires = requires.Take(searchModel.PageSize)
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
