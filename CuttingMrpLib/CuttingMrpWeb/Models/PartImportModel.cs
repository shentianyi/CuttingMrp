using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class PartImportModel
    {
        public string PartNr { get; set; }
        public int PartType { get; set; }
        public string PartDesc { get; set; }
        public int PartStatus { get; set; }
        public double MOQ { get; set; }
        public double SPQ { get; set; }
        public string Action { get; set; }
    }

    public sealed class PartCsvModelMap : CsvClassMap<PartImportModel>
    {
        public PartCsvModelMap()
        {
            Map(m => m.PartNr).Name("PartNr");
            Map(m => m.PartType).Name("PartType");
            Map(m => m.PartDesc).Name("PartDesc");
            Map(m => m.PartStatus).Name("PartStatus");
            Map(m => m.MOQ).Name("MOQ");
            Map(m => m.SPQ).Name("SPQ");
            Map(m => m.Action).Name("Action");
        }
    }
}