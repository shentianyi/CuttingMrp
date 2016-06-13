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
using OfficeOpenXml;

namespace CuttingMrpWeb.Controllers
{
    public class ProcessOrdersController : Controller
    {
        // GET: ProcessOrders
        [CustomAuthorize]
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
                    "ProceeDate","SourceQuantity","ActualQuantity","BatchQuantity","BundleQuantity","KanBanPosition","RouteNr","CompleteRate", "DerivedFrom" };
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
                    ii.Add(processOrders[i].Part.kanbanBatchQty.ToString());
                    ii.Add(processOrders[i].Part.kanbanBundleQty.ToString());
                    ii.Add(processOrders[i].Part.kanbanPosition.ToString());
                    ii.Add(processOrders[i].Part.routeNr.ToString());
                    ii.Add(Math.Round(processOrders[i].completeRate,2).ToString());
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
                List<string> head = new List<string> { " No.","Product", "PartNr", "Kanban", "PartType(KB Type)","Position", "ActualQuantity", "BundleQuantity", "BatchQuantity","KanBanPosition","RouteNr","ChangeQty" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < processOrders.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(processOrders[i].Part.productNr);
                    ii.Add(processOrders[i].partNr);
                    ii.Add(processOrders[i].Part.kanbanNrs);
                    ii.Add(processOrders[i].Part.partTypeDisplay);
                    ii.Add(processOrders[i].Part.kanbanPosition);
                    ii.Add(processOrders[i].actualQuantity.ToString());
                    ii.Add(processOrders[i].Part.kanbanBundleQty.ToString());
                    ii.Add(processOrders[i].Part.kanbanBatchQty.ToString());
                    ii.Add(processOrders[i].Part.kanbanPosition.ToString());
                    ii.Add(processOrders[i].Part.routeNr.ToString());
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
                Settings.Default.stockSourceType,
                StockMoveType.ManualEntry,
                true);

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
            int csvStartFromLine = 16;
            int excelStartFromLine = 9;
            //try
            //{
            if (forceFile == null)
            {
                throw new Exception("No file is uploaded to system");
            }
            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(forceFile.FileName));
            forceFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<CuttingOrderImportModel> records = new List<CuttingOrderImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<ProcessOrderCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;


                using (TextReader treader = System.IO.File.OpenText(filename))
                {
                    for (int i = 0; true; i++)
                    {
                        string s = treader.ReadLine();

                        if (i >= csvStartFromLine)
                        {
                            if (string.IsNullOrWhiteSpace(s))
                            {
                                break;
                            }
                            string[] fields = s.Split(char.Parse(Settings.Default.csvDelimiter));
                            records.Add(new CuttingOrderImportModel()
                            {
                                type = CuttingOrderImportModelType.White,
                                Date = fields[0],
                                Time = fields[1],
                                CuttingOrder = fields[2],
                                CuttingPosition = fields[3],
                                SingleResource = fields[4],
                                ResourceGroup = fields[5],
                                StaffNumber = fields[6],
                                WireNumber = fields[7],
                                PartNumber = fields[8],
                                KanbanNumber = fields[9],
                                CutQtyDisplay = fields[10]
                            });
                        }
                    }

                }
            }
            else if (Path.GetExtension(filename).Equals(".xlsx")) {
                FileInfo file = new FileInfo(filename);
                using (ExcelPackage ep = new ExcelPackage(file))
                {
                    ExcelWorksheet ws = ep.Workbook.Worksheets.First();
                    //string s = ws.Cells[1, 1].Value.ToString();
                    //s = ws.Cells[1, 2].ToString();
                    //int i = ws.Dimension.End.Row;
                    //  int ii = ws.Dimension.End.Row;
                    for (int i = excelStartFromLine; i <= ws.Dimension.End.Row; i++)
                    {
                        string f = ws.Cells[i, 17].Value.ToString();
                        int feedback = int.Parse(ws.Cells[i, 17].Value.ToString());
                            if (feedback > 0) {
                                records.Add(new CuttingOrderImportModel()
                                {
                                    type = CuttingOrderImportModelType.Blue,
                                    Date = ws.Cells[i, 1].Value.ToString(),
                                    Time = ws.Cells[i, 2].Value.ToString(),
                                   // CuttingOrder = fields[2],
                                   // CuttingPosition = fields[3],
                                  //  SingleResource = fields[4],
                                    ResourceGroup = ws.Cells[i, 10].Value.ToString(),
                                  //  StaffNumber = fields[6],
                                 //   WireNumber = fields[7],
                                    PartNumber = ws.Cells[i,11].Value.ToString(),
                                    KanbanNumber = ws.Cells[i, 13].Value.ToString(),
                                    CutQtyDisplay = ws.Cells[i, 17].Value.ToString()
                                });
                            }
                        }
                    }
                }


                //using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                //{ 
                //    HSSFWorkbook wk = new HSSFWorkbook(fs);
                //    ISheet sheet = wk.GetSheetAt(0);
                //    for (int j = 0; j <= sheet.LastRowNum; j++)
                //    {
                //        IRow row = sheet.GetRow(j);
                //        if (row != null)
                //        {
                //            for (int k = 0; k <= row.LastCellNum; k++)
                //            {
                //                ICell cell = row.GetCell(k);
                //                if (cell != null)
                //                {
                //                    string s = cell.ToString();
                //                    string ss = cell.ToString();
                //                }
                //            }
                //        }
                //    }

                //}
            
            bool success = true;
            List<BatchFinishOrderRecord> vr = new List<BatchFinishOrderRecord>();
            foreach (CuttingOrderImportModel r in records)
            {
                vr.Add(new BatchFinishOrderRecord()
                {
                    Id=r.Id,
                    FixOrderNr = r.KanbanNumber,
                    PartNr = r.PartNumber,
                    Amount = r.CutQty,
                    ProdTime = r.CutDateTime
                });
            }

            if (vr.Count > 0)
            {

                IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
                Hashtable results = ps.ValidateFinishOrder(vr);
                success = Settings.Default.ignoreImportKBOrderError ? true : !results.ContainsKey("WARN");
                ViewBag.Success = success;
                if (success)
                {
                    ps.BatchFinishOrder(results["SUCCESS"] as List<BatchFinishOrderRecord>, true, false);
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
            //}
            //catch (Exception ex) {
            //    throw ex;
            //    ViewBag.Msg = ex.Message;
            //    return View();
            //}
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
