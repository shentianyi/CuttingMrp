using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuttingMrpWeb.Controllers
{
    public class UnDoneStockController : Controller
    {
        // GET: UnDoneStock
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            UnDoneStockSearchModel q = new UnDoneStockSearchModel();

            IUnDoneStockService uss = new UnDoneStockService(Settings.Default.db);

            IPagedList<UnDoneStock> undonestocks = uss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetPartTypeList(null);
            SetUnDoneStockStateList(null);

            return View(undonestocks);
        }

        public ActionResult Search([Bind(Include = "PartNr, SourceType, State")] UnDoneStockSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IUnDoneStockService uss = new UnDoneStockService(Settings.Default.db);

            IPagedList<UnDoneStock> undoneStocks = uss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;
            SetPartTypeList(q.SourceType);
            SetUnDoneStockStateList(q.State);

            return View("Index", undoneStocks);
        }

        private void SetPartTypeList(int? type, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(PartType));

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
            ViewData["partTypeList"] = select;
        }

        private void SetUnDoneStockStateList(int? state, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(StockState));

            List<SelectListItem> select = new List<SelectListItem>();
            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var it in item)
            {
                if (state.HasValue && state.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }
                else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }
            ViewData["stockStateList"] = select;
        }

    }
}