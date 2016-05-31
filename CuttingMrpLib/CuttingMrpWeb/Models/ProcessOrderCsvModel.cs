using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace CuttingMrpWeb.Models
{
    public class ProcessOrderCsvModel
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string CuttingOrder { get; set; }
        public string CuttingPosition { get; set; }
        public string SingleResource { get; set; }
        public string ResourceGroup { get; set; }
        public string StaffNumber { get; set; }
        public string WireNumber { get; set; }
        public string PartNumber { get; set; }
        public string KanbanNumber { get; set; }
        public double CutQty { get {
                return double.Parse(this.CutQtyDisplay.Split(',')[0]); 
            } }
        public string CutQtyDisplay { get; set; }

        public DateTime CutDateTime {
            get
            {
                return DateTime.ParseExact(this.Date + " " + (this.Time.Length == 4 ? "0" + this.Time : this.Time), "dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture);
            }
        }
    }

    public  class ProcessOrderCsvModelMap : CsvClassMap<ProcessOrderCsvModel>
    {
        public   ProcessOrderCsvModelMap()
        {
            this.Map(m => m.Date).Name("Date");
            this.Map(m => m.Time).Name("Time");
            this.Map(m => m.CuttingOrder).Name("Cutting order");
            this.Map(m => m.CuttingPosition).Name("Cutting position");
            this.Map(m => m.SingleResource).Name("Single resource");
            this.Map(m => m.ResourceGroup).Name("Resource group");
            this.Map(m => m.StaffNumber).Name("Staff number");
            this.Map(m => m.WireNumber).Name("Wire number");
            this.Map(m => m.PartNumber).Name("Partnumber");
            this.Map(m => m.KanbanNumber).Name("Kanban number");
            this.Map(m => m.CutQtyDisplay).Name("Cut quantity");
        }
    }
}