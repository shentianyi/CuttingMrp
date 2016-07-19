using CuttingMrpLib;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using CuttingMrpWeb.Helpers;
using System.IO;
using System.Text;
using CuttingMrpWeb.Models;
using System.Collections;
using OfficeOpenXml;

namespace CuttingMrpWeb.Controllers
{
    public class StocksController : Controller
    {
        // GET: Stocks

        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            StockSearchModel q = new StockSearchModel();

            IStockService ss = new StockService(Settings.Default.db);

            IPagedList<Stock> stocks = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(stocks);
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int id)
        {
            return GetStockById(id);
        }

        // POST: Stocks/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,partNr,fifo,quantity,container,wh,position,source,sourceType")] Stock stock)
        {
            //try
            //{
            IStockService ss = new StockService(Settings.Default.db);
            ss.Update(stock);
            return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {

            return GetStockById(id);
        }

        // POST: Stocks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            //try
            //{
            IStockService ss = new StockService(Settings.Default.db);
            ss.DeleteById(id);
            return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        public ActionResult Search([Bind(Include = "PartNr,FIFOFrom,FIFOTo,QuantityFrom,QuantityTo,Wh,Position")] StockSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            ViewBag.Query = q;

            IStockService ss = new StockService(Settings.Default.db);

            IPagedList<Stock> stocks = ss.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            return View("Index", stocks);
        }

        private ActionResult GetStockById(int? id)
        {
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IStockService ss = new StockService(Settings.Default.db);
            Stock stock = ss.FindById(id.Value);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        public ActionResult SumOfStock([Bind(Include = "PartNr")] SumOfStockSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            ISumOfStockService sos = new SumOfStockService(Settings.Default.db);
            IPagedList<SumOfStock> sumOfStock = sos.SearchSumOfStock(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(sumOfStock);
        }

        public void ExportSumOfStock([Bind(Include = "PartNr")] SumOfStockSearchModel q)
        {
            ISumOfStockService sos = new SumOfStockService(Settings.Default.db);

            List<SumOfStock> processOrders = sos.SearchSumOfStock(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "PartNr", "SumOfStock" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < processOrders.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(processOrders[i].partNr);
                    ii.Add(processOrders[i].SumOfStock.ToString());
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "SumOfStocks" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();

        }

        public void Export([Bind(Include = "PartNr,FIFOFrom,FIFOTo,QuantityFrom,QuantityTo,Wh,Position")] StockSearchModel q)
        {
            IStockService ss = new StockService(Settings.Default.db);
            List<Stock> stocks = ss.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { "No.", "PartNr", "FIFO", "Quantity", "Wh", "KanBanNr", "KanBanPosition" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < stocks.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(stocks[i].partNr);
                    ii.Add(stocks[i].fifo.ToString());
                    ii.Add(stocks[i].quantity.ToString());
                    ii.Add(stocks[i].wh.ToString());
                    ii.Add(stocks[i].Part.kanbanNrs.ToString());
                    ii.Add(stocks[i].Part.kanbanPosition.ToString());
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Stocks" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
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
        public ActionResult ImportRecord(HttpPostedFileBase stockFile)
        { 
            int excelStartFromLine = 2;
            //try
            //{
            if (stockFile == null)
            {
                throw new Exception("No file is uploaded to system");
            }
            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(stockFile.FileName));
            stockFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);
   List<BatchFinishOrderRecord> vr = new List<BatchFinishOrderRecord>();

            IBatchOrderTemplateService bs = new BatchOrderTemplateService(Settings.Default.db);

            if (Path.GetExtension(filename).Equals(".xlsx"))
            {
                FileInfo file = new FileInfo(filename);
                using (ExcelPackage ep = new ExcelPackage(file))
                {
                    ExcelWorksheet ws = ep.Workbook.Worksheets.First();

                    for (int i = excelStartFromLine; i <= ws.Dimension.End.Row; i++)
                    {

                        string partNr = ws.Cells[i, 1].Value == null ? string.Empty : ws.Cells[i, 1].Value.ToString();

                        string kanbanNr = ws.Cells[i, 2].Value == null ? string.Empty : ws.Cells[i, 2].Value.ToString();
                        float qty = 0;
                        float.TryParse(ws.Cells[i, 3].Value.ToString(), out qty);
                        StockMoveType type = (StockMoveType)(int.Parse(ws.Cells[i, 4].Value.ToString()));
                        DateTime dt = DateTime.Now.Date;
                        if (ws.Cells[i, 5].Value != null)
                        {
                            DateTime.TryParse(ws.Cells[i, 5].Value.ToString(), out dt);
                        }
                        if (string.IsNullOrWhiteSpace(partNr) && (!string.IsNullOrWhiteSpace(kanbanNr)))
                        {
                            BatchOrderTemplate kb = bs.FindByNr(kanbanNr);
                            if (kb != null)
                            {
                                partNr = kb.partNr;
                                
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(partNr)) {
                            vr.Add(new BatchFinishOrderRecord()
                            {
                                Id = string.Format("{0}_{1}_{2}_{3}_{4}", partNr, kanbanNr, qty, type, dt),
                                MoveType = type,
                                PartNr = partNr,
                                Amount = qty,
                                ProdTime = dt
                            });
                        }

                    }
                }
            }

            bool success = true;
         

            if (vr.Count > 0)
            {

                IProcessOrderService ps = new ProcessOrderService(Settings.Default.db);
                 ViewBag.Success = true;
                if (success)
                {
                    ps.BatchFinishOrder(vr, true, false);
                    ViewBag.Msg = "Import Stock Success!";

                    return View();
                }
                else
                {
                    ViewBag.Msg = "Validate Warning!";
                    return View();
                }
            }
            else
            {
                ViewBag.Msg = "No Record";

                return View();
            }
        }

    }
}
