    using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class BatchOrderTemplateImportModel
    {
        public string  OrderNr { get; set; }
        public string  PartNr { get; set; }
        public double BatchQuantity { get; set; }
        public int Type { get; set; }
        public double Bundle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Operator { get; set; }
        public string Remark1 { get; set; }
        public string  Remark2 { get; set; }
        public string Action { get; set; }
    }

    public sealed class BatchOrderTemplateCsvModelMap: CsvClassMap<BatchOrderTemplateImportModel>
    {
        public BatchOrderTemplateCsvModelMap()
        {
            Map(m => m.OrderNr).Name("OrderNr");
            Map(m => m.PartNr).Name("PartNr");
            Map(m => m.BatchQuantity).Name("BatchQuantity");
            Map(m => m.Type).Name("Type");
            Map(m => m.Bundle).Name("Bundle");
            Map(m => m.CreatedAt).Name("CreatedAt");
            Map(m => m.UpdatedAt).Name("UpdatedAt");
            Map(m => m.Operator).Name("Operator");
            Map(m => m.Remark1).Name("Remark1");
            Map(m => m.Remark2).Name("Remark2");
            Map(m => m.Action).Name("Action");
        }
    }
}