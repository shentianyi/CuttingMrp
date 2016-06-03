using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuttingMrpWeb.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rate()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Data([Bind(Include ="PartNr,DateFrom,DateTo,Type")] DashboardSearchModel searchModel) {
            IDashboardService ds = new DashboardService(Properties.Settings.Default.db);

            List<DashboardItem> items = new List<DashboardItem>();
            switch (searchModel.Type)
            {
                case 100:
                    items = ds.GetPartStockDash(searchModel);
                    break;
                case 200:
                    items = ds.GetPartCompleteRateDash(searchModel);
                    break;
                default:
                    break;
            }
            return Json(items, JsonRequestBehavior.AllowGet);

        }
    }
}