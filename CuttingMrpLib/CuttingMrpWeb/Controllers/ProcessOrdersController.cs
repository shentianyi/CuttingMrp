using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using CsvHelper.Configuration;
using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
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
            SetPartTypeList(null);
            SetProcessOrderMrpRoundList(null);

            ProcessOrderInfoModel info = ps.GetProcessOrderInfo(q);
            ViewBag.Info = info;

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
        public ActionResult Edit(string id)
        {
            ProcessOrder order = GetProcessOrderById(id);

            if (order != null)
            {
                SetProcessOrderStatusList(order.status, false);
            }

            return ValidateProcessOrder(order);
        }

        // POST: ProcessOrders/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "orderNr,status,actualQuantity")] ProcessOrder order)
        {
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
            ps.Update(order);
            return RedirectToAction("Index");
        }

        // GET: ProcessOrders/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return ValidateProcessOrder(GetProcessOrderById(id));
            }
        }

        // POST: ProcessOrders/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            //try
            //{
            // TODO: Add delete logic here
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
            ps.DeleteById(id);
            return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }


        public ActionResult Search([Bind(Include = "OrderNr,SourceDoc,DerivedFrom,ProceeDateFrom,ProceeDateTo,PartNr,ActualQuantityFrom,ActualQuantityTo,CompleteRateFrom,CompleteRateTo,Status,MrpRound,KanbanNr,PartType")] ProcessOrderSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);

            IPagedList<ProcessOrder> processOrders = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetProcessOrderStatusList(q.Status);
            SetPartTypeList(q.PartType);
            SetProcessOrderMrpRoundList(q.MrpRound);

            ProcessOrderInfoModel info = ps.GetProcessOrderInfo(q);
            ViewBag.Info = info;


            return View("Index", processOrders);
        }

        public void Export([Bind(Include = "OrderNr,SourceDoc,DerivedFrom,ProceeDateFrom,ProceeDateTo,PartNr,ActualQuantityFrom,ActualQuantityTo,CompleteRateFrom,CompleteRateTo,Status,MrpRound,KanbanNr,PartType")] ProcessOrderSearchModel q)
        {
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);

            List<ProcessOrder> processOrders = ps.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "OrderNr", "PartNr", "Kanban","PartType(KB Type)","Status",
                    "ProceeDate","SourceQuantity","ActualQuantity","BatchQuantity","CompleteRate", "DerivedFrom" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for(var i=0; i<processOrders.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(processOrders[i].orderNr);
                    ii.Add(processOrders[i].partNr); 
                    ii.Add(processOrders[i].Part.kanbanNrs);
                    ii.Add(processOrders[i].Part.partTypeDisplay);
                    ii.Add(processOrders[i].statusDisplay);
                    ii.Add(processOrders[i].proceeDate.ToString());
                    ii.Add(processOrders[i].sourceQuantity.ToString());
                    ii.Add(processOrders[i].actualQuantity.ToString());
                    ii.Add(processOrders[i].batchQuantity.ToString());
                    ii.Add(processOrders[i].completeRate.ToString());
                    ii.Add(processOrders[i].derivedFrom);

                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Orders" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();

        }

        public void ExportKB([Bind(Include = "OrderNr,SourceDoc,DerivedFrom,ProceeDateFrom,ProceeDateTo,PartNr,ActualQuantityFrom,ActualQuantityTo,CompleteRateFrom,CompleteRateTo,Status,MrpRound,PartType")] ProcessOrderSearchModel q)
        {
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);

            List<ProcessOrder> processOrders = ps.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.","Product", "PartNr", "Kanban", "PartType(KB Type)","Position", "ActualQuantity", "BundleQuantity", "BatchQuantity","ChangeQty" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < processOrders.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(processOrders[i].Part.productNr);
                    ii.Add(processOrders[i].partNr);
                    ii.Add(processOrders[i].Part.kanbanNrs);
                    ii.Add(processOrders[i].Part.partTypeDisplay);

                    ii.Add(processOrders[i].Part.kanbanPositions);
                    ii.Add(processOrders[i].actualQuantity.ToString());
                    ii.Add(processOrders[i].Part.moq.ToString());
                    ii.Add(processOrders[i].Part.spq.ToString());
                    ii.Add(processOrders[i].needChangeKbQtyDisplay);
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Kanban" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();

        }

        [HttpPost]
        public ActionResult Finish()
        {
            var v = Request.Form.Get("finishOrderIds");
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
            ps.FinishOrdersByIds(v.Split(',').ToList(),
                DateTime.Now,
                Settings.Default.stockContainer,
                Settings.Default.stockWh,
                Settings.Default.stockPosition,
                Settings.Default.stockSource,
                Settings.Default.stockSourceType);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Cancel()
        {
            var v = Request.Form.Get("cancelOrderIds");
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
            ps.CancelOrdersByIds(v.Split(',').ToList(), false);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ImportForceRecord(HttpPostedFileBase forceFile)
        {
            try
            {
                string s = forceFile.FileName;
                var appData = Server.MapPath("~/TmpFile/");
                var filename = Path.Combine(appData,
                    DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(forceFile.FileName));
                forceFile.SaveAs(filename);

                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<ProcessOrderCsvModelMap>();
                List<ProcessOrderCsvModel> records = new List<ProcessOrderCsvModel>();
                using (var reader = new CsvReader(new StreamReader(filename), configuration))
                {
                    //reader.Read();
                    //var header=  reader.FieldHeaders;
                    records = reader.GetRecords<ProcessOrderCsvModel>().ToList();
                }
                bool success = true;
                if (records.Count > 0)
                {
                    List<BatchFinishOrderRecord> vr = new List<BatchFinishOrderRecord>();
                    foreach (ProcessOrderCsvModel r in records)
                    {
                        vr.Add(new BatchFinishOrderRecord()
                        {
                            FixOrderNr = r.KanbanNumber,
                            PartNr = r.PartNumber,
                            Amount =r.CutQty,
                            ProdTime = r.CutDateTime
                        });
                    }
                    IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
                    Hashtable results = ps.ValidateFinishOrder(vr);
                    success = !results.ContainsKey("WARN");
                    ViewBag.Success = success;
                    if (success)
                    {
                        ps.BatchFinishOrder(vr, true);
                        ViewBag.Msg = "Finish Success!";

                        return View();
                    }
                    else
                    {
                        ViewBag.Msg = "Validate Warning!";
                        return View(results["WARN"] as List<BatchFinishOrderRecord>);
                    }
                }
                else
                {
                    ViewBag.Msg = "No Record";

                    return View();
                }
            }
            catch (Exception ex) {
                ViewBag.Msg = ex.Message;
                return View();
            }
        }

        private ActionResult ValidateProcessOrder(ProcessOrder processOrder)
        {
            if (processOrder == null)
            {
                return HttpNotFound();
            }
            return View(processOrder);
        }

        

        private ProcessOrder GetProcessOrderById(string id)
        {
            IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
            ProcessOrder order = ps.FindById(id);
            return order;
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


        private void SetProcessOrderMrpRoundList(string mrpRound, bool allowBlank = true)
        {
            IMrpRoundService mrs = new MrpRoundService(Settings.Default.db);

            List<MrpRound> rounds = mrs.GetRecents(Settings.Default.mrpRoundSelectLimit);


            List<SelectListItem> select = new List<SelectListItem>();
            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var it in rounds)
            {
                if ((!string.IsNullOrWhiteSpace(mrpRound)) && mrpRound.Equals(it.mrpRound))
                {
                    select.Add(new SelectListItem { Text = it.mrpRound, Value = it.mrpRound.ToString(), Selected = true });
                }
                else
                {
                    select.Add(new SelectListItem { Text = it.mrpRound, Value = it.mrpRound, Selected = false });
                }
            }
            ViewData["mrpRoundSelect"] = select;
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

    }
}
