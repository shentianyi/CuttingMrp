using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpWeb.Models;

namespace CuttingMrpWeb.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard

        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rate()
        {
            return View();
        }

        public ActionResult PartStock()
        {
            return View();
        }


        [HttpGet]
        public JsonResult Data([Bind(Include ="PartNr,DateFrom,DateTo,Type,Top")] DashboardSearchModel searchModel) {
            IDashboardService ds = new DashboardService(Properties.Settings.Default.db);

            Dictionary<string, List<DashboardItem>> data = new Dictionary<string, List<DashboardItem>>();
           // List<DashboardItem> items = new List<DashboardItem>();
            switch (searchModel.Type)
            {
                case 100:
                    data = ds.GetPartStockDash(searchModel);
                    break;
                case 200:
                    data = ds.GetPartCompleteRateDash(searchModel);
                    break;
                case 300:
                    searchModel.DateFrom = DateTime.Now.Date.AddDays(-8);
                    searchModel.DateTo = DateTime.Now.Date.AddDays(-1);
                    data = ds.GetPartTopRateDash(searchModel);
                    break;
                default:
                    break;
            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}