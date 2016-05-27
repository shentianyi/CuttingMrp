using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuttingMrpWeb.Controllers
{
    public class RequirementsController : Controller
    {
        // GET: Requirements
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            RequirementSearchModel q = new RequirementSearchModel();
            IRequirementService rs = new RequirementService(Settings.Default.db);

            IPagedList<Requirement> requirements = rs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(requirements);
        }

        // GET: Requirements/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Requirements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Requirements/Create
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

        // GET: Requirements/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Requirements/Edit/5
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

        // GET: Requirements/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Requirements/Delete/5
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


        public ActionResult Search([Bind(Include = "PartNr,OrderedDateFrom,OrderedDateTo,RequiredTimeFrom,RequiredTimeTo,QuantityFrom,QuantityTo,Status,DerivedFrom,DerivedType")] RequirementSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IRequirementService rs = new RequirementService(Settings.Default.db);
            IPagedList<Requirement> requirements = rs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;
            
            return View("Index", requirements);
        }



    }
}
