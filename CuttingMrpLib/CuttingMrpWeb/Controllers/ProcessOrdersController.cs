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
    public class ProcessOrdersController : Controller
    {
        // GET: ProcessOrders
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            ProcessOrderSearchModel q = new ProcessOrderSearchModel();

            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);

            IPagedList<ProcessOrder> processOrders = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetProcessOrderStatusList(null);

            return View(processOrders);
        }
        // GET: ProcessOrders/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProcessOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProcessOrders/Create
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

        // GET: ProcessOrders/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProcessOrders/Edit/5
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

        // GET: ProcessOrders/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProcessOrders/Delete/5
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


        public ActionResult Search([Bind(Include = "OrderNr,SourceDoc,DerivedFrom,ProceeDateFrom,ProceeDateTo,PartNr,ActualQuantityFrom,ActualQuantityTo,CompleteRateFrom,CompleteRateTo,Status")] ProcessOrderSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);

            IPagedList<ProcessOrder> processOrders = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);
             
            ViewBag.Query = q;

            SetProcessOrderStatusList(q.Status);
            return View("Index", processOrders);
        }


        private void SetProcessOrderStatusList(int? status, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(ProcessOrderStatus));

            List<SelectListItem> select = new List<SelectListItem>();
            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var it in item)
            {
                if (status.HasValue && status.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }
                else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }
            ViewData["statusList"] = select;
        }
    }
}
