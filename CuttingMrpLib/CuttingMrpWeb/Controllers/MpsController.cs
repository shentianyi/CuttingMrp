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