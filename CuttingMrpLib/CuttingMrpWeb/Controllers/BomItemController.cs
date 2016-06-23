using CuttingMrpWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Models;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;

namespace CuttingMrpWeb.Controllers
{
    public class BomItemController : Controller
    {
        // GET: BomItem
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            BomItemSearchModel q = new BomItemSearchModel();

            IBomItemService bis = new BomItemService(Settings.Default.db);

            IPagedList<BomItem> bomItems = bis.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(bomItems);
        }

        public ActionResult Search([Bind(Include = "ComponentId, VFFrom, VFTo, VTFrom, VTTo, BomId")] BomItemSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IBomItemService bis = new BomItemService(Settings.Default.db);

            IPagedList<BomItem> bomItems = bis.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View("Index", bomItems);
        }

        public void Export([Bind(Include = "ID, ComponentId, VFFrom, VFTo, VTFrom, VTTo, BomId")] BomItemSearchModel q)
        {
            IBomItemService ps = new BomItemService(Settings.Default.db);

            List<BomItem> bomItems = ps.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "ID","ComponentId", "ValidFrom", "ValidTo", "HasChild", "UOM", "Quantity", "BomId"};
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < bomItems.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(bomItems[i].id.ToString());
                    ii.Add(bomItems[i].componentId);
                    ii.Add(bomItems[i].validFrom.ToString());
                    ii.Add(bomItems[i].validTo.ToString());
                    ii.Add(bomItems[i].hasChind.ToString());
                    ii.Add(bomItems[i].uom.ToString());
                    ii.Add(bomItems[i].quantity.ToString());
                    ii.Add(bomItems[i].bomId);
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "BomItem" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

        //public ActionResult ImportBomItemRecord(HttpPostedFileBase bomItemFile)
        //{
        //    if (bomItemFile == null)
        //    {
        //        throw new Exception("No file is uploaded to system");
        //    }

        //    var appData = Server.MapPath("~/TmpFile/");
        //    var filename = Path.Combine(appData,
        //        DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(bomItemFile.FileName));

        //    bomItemFile.SaveAs(filename);
        //    string ex = Path.GetExtension(filename);

        //    List<BomItemImportModel> records = new List<BomItemImportModel>();

        //    if (ex.Equals(".csv"))
        //    {
        //        CsvConfiguration configuration = new CsvConfiguration();
        //        configuration.Delimiter = Settings.Default.csvDelimiter;
        //        configuration.HasHeaderRecord = true;
        //        configuration.SkipEmptyRecords = true;
        //        configuration.RegisterClassMap<BomItemCsvModelMap>();
        //        configuration.TrimHeaders = true;
        //        configuration.TrimFields = true;

        //        try
        //        {
        //            using (TextReader treader = System.IO.File.OpenText(filename))
        //            {
        //                CsvReader csvReader = new CsvReader(treader, configuration);
        //                records = csvReader.GetRecords<BomItemImportModel>().ToList();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewBag.Msg = "Import Failure!" + e;
        //        }

        //        List<string> CreateErrorList = new List<string>();
        //        List<string> UpdateErrorList = new List<string>();

        //        if (records.Count > 0)
        //        {
        //            int AllQty = records.Count;
        //            int CreateSuccessQty = 0;
        //            int CreateFailureQty = 0;
        //            int UpdateSuccessQty = 0;
        //            int UpdateFailureQty = 0;
        //            int OtherQty = 0;

        //            IBomItemService ps = new BomItemService(Settings.Default.db);

        //            foreach (BomItemImportModel record in records)
        //            {
        //                if (string.IsNullOrWhiteSpace(record.Action))
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    //新建
        //                    BomItem bomItem= new BomItem()
        //                    {
        //                        componentId = record.ComponentId,
        //                        validFrom = record.ValidFrom,
        //                        validTo = record.ValidTo,
        //                        hasChind =Convert.ToByte(record.HasChild),
        //                        uom = record.UOM,
        //                        quantity = record.Quantity,
        //                        bomId = record.BomId
        //                    };

        //                    if (record.Action.Trim().Equals("create"))
        //                    {
        //                        try
        //                        {
        //                            bool result = ps.Create(bomItem);
        //                            if (result)
        //                            {
        //                                CreateSuccessQty++;
        //                            }
        //                            else
        //                            {
        //                                CreateFailureQty++;

        //                                ViewBag.MsgCreate = "Some Part Create Failure:";

        //                                CreateErrorList.Add(bomItem.id);
        //                                ViewData["createError"] = CreateErrorList;
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            ViewBag.MsgCreate = "Create Failure!" + e;
        //                        }
        //                    }
        //                    else if (record.Action.Trim().Equals("update"))
        //                    {
        //                        //更新
        //                        try
        //                        {
        //                            bool result = ps.Update(bomItem);
        //                            if (result)
        //                            {
        //                                UpdateSuccessQty++;
        //                            }
        //                            else
        //                            {
        //                                UpdateFailureQty++;

        //                                ViewBag.MsgUpdate = "Some Part Update Failure:";

        //                                UpdateErrorList.Add(bomItem.partNr);
        //                                ViewData["updateError"] = UpdateErrorList;
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            ViewBag.MsgUpdate = "Update Failure!" + e;
        //                        }
        //                    }
        //                    else if (record.Action.Trim().Equals("delete"))
        //                    {
        //                        //删除  忽略
        //                    }
        //                    else
        //                    {
        //                        //错误 忽略
        //                    }
        //                }
        //            }

        //            OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty;
        //            Dictionary<string, int> Qty = new Dictionary<string, int>();
        //            Qty.Add("AllQty", AllQty);
        //            Qty.Add("CreateSuccessQty", CreateSuccessQty);
        //            Qty.Add("CreateFailureQty", CreateFailureQty);
        //            Qty.Add("UpdateSuccessQty", UpdateSuccessQty);
        //            Qty.Add("UpdateFailureQty", UpdateFailureQty);
        //            Qty.Add("OtherQty", OtherQty);
        //            ViewData["Qty"] = Qty;
        //        }
        //        else
        //        {
        //            ViewBag.NoData = "There are no Data.Please Check Delimiter.";
        //        }
        //    }

        //    return View();
        //}
    }
}