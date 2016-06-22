using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
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

        [CustomAuthorize]
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
        public ActionResult Edit(int? id)
        {
            Requirement requirement = GetRequirementById(id.Value);

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
            //try
            //{
                // TODO: Add update logic here
                IRequirementService rs = new RequirementService(Settings.Default.db);
                rs.Update(requirement);
                return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: Requirements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                return ValidateRequirement(GetRequirementById(id.Value));
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Requirements/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            //try
            //{
                // TODO: Add delete logic here
                IRequirementService rs = new RequirementService(Settings.Default.db);
                rs.DeleteById(id);
                return RedirectToAction("Index"); 
            //}
            //catch
            //{
            //    return View();
            //}
        }


        public ActionResult Search([Bind(Include = "PartNr,OrderedDateFrom,OrderedDateTo,RequiredTimeFrom,RequiredTimeTo,QuantityFrom,QuantityTo,Status,DerivedFrom,DerivedType,OrderNr")] RequirementSearchModel q)
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

        public ActionResult Statistics([Bind(Include = "StatisticsType")] RequirementStatisticsSearchModel q)
        {
            if (string.IsNullOrWhiteSpace(q.StatisticsType))
            {
                q.StatisticsType = "DERIVEDTYPE";
            }
            IRequirementService rs = new RequirementService(Settings.Default.db);
            List<RequirementStatistics> reqs = rs.SearchStatistics(q);

            ViewBag.Query = q;
            SetSearchTypeList(q.StatisticsType);

            return View(reqs);
        }

        [HttpPost]
        public ActionResult RunMrp()
        {
            // [Bind(Include = "OrderType,MergeMethod")]
            CalculateSetting setting = new CalculateSetting()
            {
                TaskType="MRP",
                OrderType = Request.Form.Get("OrderType"),
                PartType= (PartType)int.Parse( Request.Form.Get("PartType")),
                MergeMethod = new MergeMethod()
                {
                    FirstDay = DateTime.Parse(Request.Form.Get("FirstDay")),
                    Count = int.Parse(Request.Form.Get("Count")),
                    MergeType = Request.Form.Get("MergeType")
                }
            };
            Message msg = new Message() { Result = false };
            try
            {
                ICalculateService cs = new CalculateService(Settings.Default.db);
                cs.Start(Settings.Default.mrpQueue, setting);
                msg.Result = true;
                msg.Msg = "MRP 任务运行成功!";
            }
            catch (Exception e)
            {
                msg.Msg = e.Message;
            }
            return Json(msg);
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

        private void SetSearchTypeList(string type, bool allowBlank = false)
        {
            List<EnumItem> item = RequirementStatisticsSearchModel.typeList;


            List<SelectListItem> select = new List<SelectListItem>();
            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var it in item)
            {
                if ((!string.IsNullOrWhiteSpace(type)) && type.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }
                else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }
            ViewData["typeList"] = select;
        }

    }
}
