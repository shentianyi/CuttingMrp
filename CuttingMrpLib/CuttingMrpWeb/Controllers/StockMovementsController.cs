using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Properties;
using MvcPaging;

namespace CuttingMrpWeb.Controllers
{
    public class StockMovementsController : Controller
    {
        // GET: StockMovements
        public ActionResult Index()
        {
            return View();
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
                // TODO: Add insert logic here

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
                // TODO: Add update logic here

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
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult JsonSearch([Bind(Include = "PartNr,DateFrom,DateTo")] StockMovementSearchModel q)
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
    }
}
