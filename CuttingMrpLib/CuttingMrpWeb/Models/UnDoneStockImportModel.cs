using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class UnDoneStockImportModel
    {
        public string PartNr { get; set; }
        public string KanbanNr { get; set; }
        public string Quantity { get; set; }
        public int  SourceType { get; set; }
        public int  State { get; set; }

        public int CutQuantity { get
            {
                return int.Parse(this.Quantity.Split(',')[0]);
            } }
    }

    public class UnDoneStockCsvModelMap : CsvClassMap<UnDoneStockImportModel>
    {
        public UnDoneStockCsvModelMap()
        {
            this.Map(m => m.Quantity).Name("Kanban quantity");
            this.Map(m => m.KanbanNr).Name("Kanban number");
        }
    }
}