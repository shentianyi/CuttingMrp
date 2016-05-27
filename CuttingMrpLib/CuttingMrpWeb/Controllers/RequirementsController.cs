﻿using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            SetRequirementStatusList(null);
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
            Requirement requirement = GetRequirementById(id);

            if (requirement != null)
            {
                SetRequirementStatusList(requirement.status,false);
            }

            return ValidateRequirement(requirement);
        }

        // POST: Requirements/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,partNr,orderedDate,requiredDate,quantity,status,derivedFrom,derivedType")] Requirement requirement)
        {
            try
            {
                // TODO: Add update logic here
                IRequirementService rs = new RequirementService(Settings.Default.db);
                rs.Update(requirement);
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
            return ValidateRequirement(GetRequirementById(id));
        }

        // POST: Requirements/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                IRequirementService rs = new RequirementService(Settings.Default.db);
                rs.DeleteById(id);
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

            SetRequirementStatusList(q.Status);
            return View("Index", requirements);
        }

        public ActionResult RunMrp() {

            ICalculateService cs = new CalculateService(Settings.Default.db);
            cs.Start(Settings.Default.mrpQueue, null);

            return null;
        }

        private ActionResult ValidateRequirement(Requirement requirement)
        {
            if (requirement == null)
            {
                return HttpNotFound();
            }
            return View(requirement);
        }

        private Requirement GetRequirementById(int id) {
            IRequirementService rs = new RequirementService(Settings.Default.db);
            Requirement requirement = rs.FindById(id);
            return requirement;
        }

        private void SetRequirementStatusList(int? status,bool allowBlank=true) {
            List<EnumItem> item = EnumUtility.GetList(typeof(RequirementStatus));
           
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
                else {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }
            ViewData["statusList"] = select;
        }

    }
}
