Imports CuttingMrpLib
Imports Repository

Public Class PartRepository
    Inherits Repository(Of Part)
    Implements IPartRepository

    Private _context As CuttingMrpDataContext
    Public Sub New(dc As IDataContextFactory)
        MyBase.New(dc)
        _context = Me._dataContextFactory.Context
    End Sub

    Public Function GetParents(partNr As String, validFrom As Date, validTo As Date) As List(Of Part) Implements IPartRepository.GetParents
        'Dim parts As List(Of Part) = (From p In _context.Parts
        '                              From b In p.BOMs
        '                              From bi In b.BomItems
        '                              Where bi.componentId.Equals(partNr) And b.validTo <= validTo And b.validFrom >= validFrom
        '                              Order By p.partNr
        '                              Select p).
        'Dim q = From p In _context.Parts
        '        From b In p.BOMs
        '        From bi In b.BomItems
        '        Where bi.componentId.Equals(partNr) And b.validTo >= validTo And b.validFrom <= validFrom
        '        Order By p.partNr
        '        Select p
        Dim parts As List(Of Part) = (From p In _context.Parts
                                      From b In p.BOMs
                                      From bi In b.BomItems
                                      Where bi.componentId.Equals(partNr) And b.validTo >= validTo And b.validFrom <= validFrom
                                      Order By p.partNr
                                      Select p).ToList

        Return parts
    End Function
End Class
