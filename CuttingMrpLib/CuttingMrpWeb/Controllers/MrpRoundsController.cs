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
    public class MrpRoundsController : Controller
    {
        // GET: MrpRounds
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            MRPSearchModel q = new MRPSearchModel();
            ICalculateService cs = new CalculateService(Settings.Default.db);

            IPagedList<MrpRound> rounds = cs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;
            SetMrpRoundStatusList(null);
            return View(rounds);
        }

        // GET: MrpRounds/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MrpRounds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MrpRounds/Create
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

        // GET: MrpRounds/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MrpRounds/Edit/5
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

        // GET: MrpRounds/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MrpRounds/Delete/5
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

        public ActionResult Search([Bind(Include = "MrpRoundId,RunningStatus,TimeFrom,TimeTo,Launcher")] MRPSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            ICalculateService cs = new CalculateService(Settings.Default.db);

            IPagedList<MrpRound> rounds = cs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;
            SetMrpRoundStatusList(null);


            return View("Index", rounds);
        }


        private void SetMrpRoundStatusList(int? status, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(CalculatorStatus));

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
