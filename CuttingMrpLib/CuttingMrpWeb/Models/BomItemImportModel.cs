using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class BomItemImportModel
    {
        public string ID { get; set; }
        public string  ComponentId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int HasChild { get; set; }
        public int UOM { get; set; }
        public double Quantity { get; set; }
        public string BomId { get; set; }
        public string Action { get; set; }
    }

    public sealed class BomItemCsvModelMap : CsvClassMap<BomItemImportModel>
    {
        public BomItemCsvModelMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.ComponentId).Name("ComponentId");
            Map(m => m.ValidFrom).Name("ValidFrom");
            Map(m => m.ValidTo).Name("ValidTo");
            Map(m => m.HasChild).Name("HasChild");
            Map(m => m.UOM).Name("UOM");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.BomId).Name("BomId");
            Map(m => m.Action).Name("Action");
        }
    }

}