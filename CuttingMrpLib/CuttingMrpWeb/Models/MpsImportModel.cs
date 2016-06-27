using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class MpsImportModel
    {
        public string ID { get; set; }
        public string PartNr { get; set; }
        public DateTime OrderedDate { get; set;}
        public DateTime RequiredDate { get; set; }
        public double Quantity { get; set; }
        public string Source { get; set; }
        public string SourceDoc { get; set; }
        public int Status { get; set; }
        public string UnitId { get; set; }
        public string Action { get; set; }
    }

    public sealed class MpsCsvModelMap : CsvClassMap<MpsImportModel>
    {
        public MpsCsvModelMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.PartNr).Name("PartNr");
            Map(m => m.OrderedDate).Name("OrderedDate");
            Map(m => m.RequiredDate).Name("RequiredDate");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.Source).Name("Source");
            Map(m => m.SourceDoc).Name("SourceDoc");
            Map(m => m.Status).Name("Status");
            Map(m => m.UnitId).Name("UnitId");
            Map(m => m.Action).Name("Action");
        }
    }
}