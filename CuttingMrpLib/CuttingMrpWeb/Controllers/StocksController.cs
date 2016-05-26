using CuttingMrpLib;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace CuttingMrpWeb.Controllers
{
    public class StocksController : Controller
    {
        // GET: Stocks
        public ActionResult Index(int? page)
        {
            int pageIndex = page.HasValue ? (page.Value <= 0 ? 0 : page.Value - 1) : 0;

            StockSearchModel q = new StockSearchModel();

            IStockService ss = new StockService(Settings.Default.db);

            IPagedList<Stock> stocks = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(stocks);
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int id)
        {
            return GetStockById(id);
        }

        // POST: Stocks/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,partNr,fifo,quantity,container,wh,position,source,sourceType")] Stock stock)
        {
            try
            {
                // TODO: Add update logic here
                Stock s = stock;
                IStockService ss = new StockService(Settings.Default.db);
                ss.Update(s);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            
            return GetStockById(id);
        }

        // POST: Stocks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                IStockService ss = new StockService(Settings.Default.db);
                ss.DeleteById(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search()
        {
            StockSearchModel q = new StockSearchModel()
            {
                PartNr = Request.QueryString.Get("PartNr"),
                Wh = Request.QueryString.Get("Wh"),
                Position=Request.QueryString.Get("Position")
            };

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("FIFOFrom"))) { q.FIFOFrom = DateTime.Parse(Request.QueryString.Get("FIFOFrom")); }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("FIFOTo"))) { q.FIFOTo = DateTime.Parse(Request.QueryString.Get("FIFOTo")); }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("QuantityFrom"))) { q.QuantityFrom = float.Parse(Request.QueryString.Get("QuantityFrom")); }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("QuantityTo"))) { q.QuantityTo = float.Parse(Request.QueryString.Get("QuantityTo")); }


            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = pageIndex <= 0 ? 0 : pageIndex - 1;

            ViewBag.Query = q;

            IStockService ss = new StockService(Settings.Default.db);

            IPagedList<Stock> stocks = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            return View("Index", stocks);
        }

        private ActionResult GetStockById(int? id) {
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IStockService ss = new StockService(Settings.Default.db);
            Stock stock = ss.FindById(id.Value);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }
    }
}
