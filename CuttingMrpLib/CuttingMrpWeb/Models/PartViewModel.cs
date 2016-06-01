using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuttingMrpWeb.Models
{
    public class PartViewModel
    {
        public string partNr { get; set; }
        public string partTypeDisplay { get; set; }
        public string partDesc { get; set; }
        public double? moq { get; set; }
        public double? spq { get; set; }
        public string kanbanNr { get; set;}

        public string position { get; set; }
    }
}