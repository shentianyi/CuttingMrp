using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System.IO;
using System.Text;

namespace CuttingMrpWeb.Controllers
{
    public class StockMovementsController : Controller
    {
        // GET: StockMovements
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            StockMovementSearchModel q = new StockMovementSearchModel();
            IStockMovementService ss = new StockMovementService(Settings.Default.db);

            IPagedList<StockMovement> stockMovements = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetMoveTypeDisplayList(null);

            return View(stockMovements);
        }

        // GET: StockMovements/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StockMovements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockMovements/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StockMovements/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StockMovements/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StockMovements/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StockMovements/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult JsonSearch([Bind(Include = "PartNr, DateFrom, DateTo, MoveType")] StockMovementSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);
            if (q.DateFrom.HasValue) {
                q.DateFrom = q.DateFrom.Value.Date;
            } 
            if (q.DateTo.HasValue)
            {
                q.DateTo = q.DateTo.Value.Date.AddDays(1).AddMilliseconds(-1);
            }

            ViewBag.Query = q;

            IStockMovementService ss = new StockMovementService(Settings.Default.db);

            IPagedList<StockMovement> moves = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            return Json(moves.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search([Bind(Include = "PartNr, DateFrom, DateTo, MoveType")] StockMovementSearchModel q) {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IStockMovementService ss = new StockMovementService(Settings.Default.db);
            IPagedList<StockMovement> stockMovements = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);
            
            ViewBag.Query = q;

            SetMoveTypeDisplayList(q.MoveType);
            return View("Index", stockMovements);
        }

        public void Export([Bind(Include = "PartNr, Quantity, FIFO, MoveTypeDisplay, SourceDoc, CreatedDisplay")] StockMovementSearchModel q)
        {
            IStockMovementService sms = new StockMovementService(Settings.Default.db);
            List<StockMovement> stockMovements = sms.Search(q).ToList();
            ViewBag.Query = q;
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string>
                {
                    "N0.", "PartNr", "Quantity", "FIFO", "MoveTypeDisplay", "SouceDoc", "CreatedAtDisplay"
                };

                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));

                for(var i =0; i< stockMovements.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(stockMovements[i].partNr);
                    ii.Add(stockMovements[i].quantity.ToString());
                    ii.Add(stockMovements[i].fifoDisplay.ToString());
                    ii.Add(stockMovements[i].typeDisplay.ToString());
                    ii.Add(stockMovements[i].sourceDoc.ToString());
                    ii.Add(stockMovements[i].createdAtDisplay.ToString());
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
            }

            var filename = "StockMovements" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }


        private void SetMoveTypeDisplayList(int? type, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(StockMoveType));

            List<SelectListItem> select = new List<SelectListItem>();
            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var it in item)
            {
                if (type.HasValue && type.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }
                else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }
            ViewData["moveTypeList"] = select;
        }
    }
}
