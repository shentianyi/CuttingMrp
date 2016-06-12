using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpWeb.Models;
using MvcPaging;
using CuttingMrpWeb.Helpers;
using System.IO;
using System.Text;

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

        public ActionResult StockReport(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            DashboardSearchModel q = new DashboardSearchModel();

            IStockSumRecordService sss = new StockSumRecordService(Properties.Settings.Default.db);

            IPagedList<StockSumRecord> stockSumRecord = sss.SearchStockReport(q).ToPagedList(pageIndex, Properties.Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(stockSumRecord);
        }


        public ActionResult SearchStockReports([Bind(Include = "PartNr, DateFrom, DateTo")] DashboardSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IStockSumRecordService sss = new StockSumRecordService(Properties.Settings.Default.db);

            IPagedList<StockSumRecord> stockSumRecord = sss.SearchStockReport(q).ToPagedList(pageIndex, Properties.Settings.Default.pageSize);

            ViewBag.Query = q;

            return View("StockReport", stockSumRecord);
        }

        public void ExportStockReport([Bind(Include = "PartNr, DateFrom, DateTo")] DashboardSearchModel q)
        {
            IStockSumRecordService sss = new StockSumRecordService(Properties.Settings.Default.db);

            List<StockSumRecord> stockSumRecords = sss.SearchStockReport(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "PartNr", "Quantity", "Date"};
                sw.WriteLine(string.Join(Properties.Settings.Default.csvDelimiter, head));
                for (var i = 0; i < stockSumRecords.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(stockSumRecords[i].partNr);
                    ii.Add(stockSumRecords[i].quantity.ToString());
                    ii.Add(stockSumRecords[i].date.ToString());
                    sw.WriteLine(string.Join(Properties.Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "StockReport" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();

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