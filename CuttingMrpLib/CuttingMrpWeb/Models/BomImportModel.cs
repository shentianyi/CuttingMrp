using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class BomImportModel
    {
        public string ID { get; set; }
        public string PartNr{get;set;}
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string VersionId { get; set; }
        public string BomDesc { get; set; }
        public string Action { get; set; }
    }

    public sealed class BomCsvModelMap : CsvClassMap<BomImportModel>
    {
        public BomCsvModelMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.PartNr).Name("PartNr");
            Map(m => m.ValidFrom).Name("ValidFrom");
            Map(m => m.ValidTo).Name("ValidTo");
            Map(m => m.VersionId).Name("VersionId");
            Map(m => m.BomDesc).Name("BomDesc");
            Map(m => m.Action).Name("Action");
        }
    }
}