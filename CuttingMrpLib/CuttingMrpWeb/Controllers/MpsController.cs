using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Properties;
using MvcPaging;

namespace CuttingMrpWeb.Controllers
{
    [CustomAuthorize]
    public class MpsController : Controller
    {
        // GET: Mps
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            MpsSeachModel q = new MpsSeachModel();
            IMpsService ms = new MpsService(Settings.Default.db);

            IPagedList<MP> mps = ms.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetMpStatusDisplayList(null);
            return View(mps);
        }

        public ActionResult Search([Bind(Include = "PartNr, OrderedDateFrom, OrderedDateTo,RequiredDateFrom, RequiredDateTo, Status")] MpsSeachModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IMpsService ms = new MpsService(Settings.Default.db);
            IPagedList<MP> mps = ms.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetMpStatusDisplayList(q.Status);

            return View("Index", mps);
        }

        // GET: Mps/Create
        public ActionResult Create()
        {
            SetMpStatusDisplayList(null);
            return View();
        }

        // POST: Mps/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "id, partnr, orderedDate, requiredDate, quantity, source, sourceDoc,status,unitId")] MP mp)
        {
            try
            {
                // TODO: Add insert logic here
                IMpsService mps = new MpsService(Settings.Default.db);
                
                mps.Create(mp);

                return RedirectToAction("Index");
            }
            catch
            {
                if (String.IsNullOrWhiteSpace(mp.source))
                {
                    ViewBag.SourceError = "Source 字段是必需的。";
                }

                if (String.IsNullOrWhiteSpace(mp.partnr))
                {
                    ViewBag.Error = "PartNr 字段是必需的。";
                }
                else
                {
                    ViewBag.Error = "PartNr 不存在。";
                }

                SetMpStatusDisplayList(null);
                return View();
            }
        }

        // GET: Mps/Edit/5
        public ActionResult Edit(string id)
        {
            MP mps = GetMpById(id);

            if (mps != null)
            {
                SetMpStatusDisplayList(mps.status);
            }

            return ValidateMps(mps);
        }

        // POST: ProcessOrders/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id, requiredDate, quantity, source, sourceDoc")] MP mp)
        {
            IMpsService mps = new MpsService(Settings.Default.db);
            mps.Update(mp);
            return RedirectToAction("Index");
        }

        // GET: Mps/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return ValidateMps(GetMpById(id));
            }
        }

        // POST: Mps/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            //try
            //{
            // TODO: Add delete logic here
            IMpsService mps = new MpsService(Settings.Default.db);
            mps.DeleteById(id);
            return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        private ActionResult ValidateMps(MP mps)
        {
            if (mps == null)
            {
                return HttpNotFound();
            }

            return View(mps);
        }

        private MP GetMpById(string id)
        {
            IMpsService ps = new MpsService(Settings.Default.db);
            MP mp = ps.FindById(id);
            return mp;
        }

        private void SetMpStatusDisplayList(int? type, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(MPSStatus));

            List<SelectListItem> select = new List<SelectListItem>();

            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }

            foreach(var it in item)
            {
                if(type.HasValue&& type.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }

            ViewData["statusList"] = select;
        }
    }
}