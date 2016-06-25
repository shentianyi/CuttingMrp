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

        public ActionResult Search([Bind(Include = "ID, ComponentId, VFFrom, VFTo, VTFrom, VTTo, BomId")] BomItemSearchModel q)
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
                List<string> head = new List<string> { " No.", "ID","ComponentId", "ValidFrom", "ValidTo", "HasChild", "UOM", "Quantity", "BomId", "Action"};
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
                    ii.Add("");
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

        public ActionResult ImportBomItemRecord(HttpPostedFileBase bomItemFile)
        {
            if (bomItemFile == null)
            {
                //TODO: BomItem Import 优化
                throw new Exception("No file is uploaded to system");
            }

            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(bomItemFile.FileName));

            bomItemFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<BomItemImportModel> records = new List<BomItemImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<BomItemCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;

                try
                {
                    using (TextReader treader = System.IO.File.OpenText(filename))
                    {
                        CsvReader csvReader = new CsvReader(treader, configuration);
                        records = csvReader.GetRecords<BomItemImportModel>().ToList();
                    }
                }
                catch (Exception e)
                {
                    ViewBag.TextExpMsg = "<-------------Read Csv File Exception!,Please Check.------------->" + e;
                }

                List<Dictionary<string, string>> CreateErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> UpdateErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> DeleteErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> ActionNullErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> OtherErrorDic = new List<Dictionary<string, string>>();

                if (records.Count > 0)
                {
                    int AllQty = records.Count;
                    int CreateSuccessQty = 0;
                    int CreateFailureQty = 0;
                    int UpdateSuccessQty = 0;
                    int UpdateFailureQty = 0;
                    int DeleteSuccessQty = 0;
                    int DeleteFailureQty = 0;
                    int ActionNullQty = 0;
                    int OtherQty = 0;

                    IBomItemService ps = new BomItemService(Settings.Default.db);

                    foreach (BomItemImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            ActionNullQty++;

                            Dictionary<string, string> ActionNullErrorList = new Dictionary<string, string>();

                            ActionNullErrorList.Add("ID", record.ID);
                            ActionNullErrorList.Add("ComponentId", record.ComponentId);
                            ActionNullErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                            ActionNullErrorList.Add("ValidTo", record.ValidTo.ToString());
                            ActionNullErrorList.Add("HasChild", record.HasChild.ToString());
                            ActionNullErrorList.Add("UOM", record.UOM.ToString());
                            ActionNullErrorList.Add("Quantity", record.Quantity.ToString());
                            ActionNullErrorList.Add("BomId", record.BomId);
                            ActionNullErrorList.Add("Action", record.Action);

                            ActionNullErrorDic.Add(ActionNullErrorList);
                            ViewData["actionNullErrorDic"] = ActionNullErrorDic;
                        }
                        else
                        {
                            //新建
                            BomItem bomItem = new BomItem()
                            {
                                id =Convert.ToInt32(record.ID),
                                componentId = record.ComponentId,
                                validFrom = record.ValidFrom,
                                validTo = record.ValidTo,
                                hasChind = Convert.ToByte(record.HasChild),
                                uom = record.UOM,
                                quantity = record.Quantity,
                                bomId = record.BomId
                            };

                            if (record.Action.Trim().ToLower().Equals("create"))
                            {
                                try
                                {
                                    bool result = ps.Create(bomItem);
                                    if (result)
                                    {
                                        CreateSuccessQty++;
                                    }
                                    else
                                    {
                                        CreateFailureQty++;

                                        Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();

                                        CreateErrorList.Add("ID", record.ID);
                                        CreateErrorList.Add("ComponentId", record.ComponentId);
                                        CreateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                        CreateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                        CreateErrorList.Add("HasChild", record.HasChild.ToString());
                                        CreateErrorList.Add("UOM", record.UOM.ToString());
                                        CreateErrorList.Add("Quantity", record.Quantity.ToString());
                                        CreateErrorList.Add("BomId", record.BomId);
                                        CreateErrorList.Add("Action", record.Action);

                                        CreateErrorDic.Add(CreateErrorList);
                                        ViewData["createErrorDic"] = CreateErrorDic;
                                    }
                                }
                                catch (Exception)
                                {
                                    CreateFailureQty++;

                                    Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();

                                    CreateErrorList.Add("ID", record.ID);
                                    CreateErrorList.Add("ComponentId", record.ComponentId);
                                    CreateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    CreateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    CreateErrorList.Add("HasChild", record.HasChild.ToString());
                                    CreateErrorList.Add("UOM", record.UOM.ToString());
                                    CreateErrorList.Add("Quantity", record.Quantity.ToString());
                                    CreateErrorList.Add("BomId", record.BomId);
                                    CreateErrorList.Add("Action", record.Action);

                                    CreateErrorDic.Add(CreateErrorList);
                                    ViewData["createErrorDic"] = CreateErrorDic;
                                }
                            }
                            else if (record.Action.Trim().ToLower().Equals("update"))
                            {
                                //更新
                                try
                                {
                                    bool result = ps.Update(bomItem);
                                    if (result)
                                    {
                                        UpdateSuccessQty++;
                                    }
                                    else
                                    {
                                        UpdateFailureQty++;

                                        Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();

                                        UpdateErrorList.Add("ID", record.ID);
                                        UpdateErrorList.Add("ComponentId", record.ComponentId);
                                        UpdateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                        UpdateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                        UpdateErrorList.Add("HasChild", record.HasChild.ToString());
                                        UpdateErrorList.Add("UOM", record.UOM.ToString());
                                        UpdateErrorList.Add("Quantity", record.Quantity.ToString());
                                        UpdateErrorList.Add("BomId", record.BomId);
                                        UpdateErrorList.Add("Action", record.Action);

                                        UpdateErrorDic.Add(UpdateErrorList);
                                        ViewData["updateErrorDic"] = UpdateErrorDic;
                                    }
                                }
                                catch (Exception)
                                {
                                    UpdateFailureQty++;
                                    Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();

                                    UpdateErrorList.Add("ID", record.ID);
                                    UpdateErrorList.Add("ComponentId", record.ComponentId);
                                    UpdateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    UpdateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    UpdateErrorList.Add("HasChild", record.HasChild.ToString());
                                    UpdateErrorList.Add("UOM", record.UOM.ToString());
                                    UpdateErrorList.Add("Quantity", record.Quantity.ToString());
                                    UpdateErrorList.Add("BomId", record.BomId);
                                    UpdateErrorList.Add("Action", record.Action);

                                    UpdateErrorDic.Add(UpdateErrorList);
                                    ViewData["updateErrorDic"] = UpdateErrorDic;
                                }
                            }
                            else if (record.Action.Trim().ToLower().Equals("delete"))
                            {
                                //删除
                                try
                                {
                                    bool result = ps.Delete(bomItem);

                                    if (result)
                                    {
                                        DeleteSuccessQty++;
                                    }
                                    else
                                    {
                                        DeleteFailureQty++;
                                        Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                        DeleteErrorList.Add("ID", record.ID);
                                        DeleteErrorList.Add("ComponentId", record.ComponentId);
                                        DeleteErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                        DeleteErrorList.Add("ValidTo", record.ValidTo.ToString());
                                        DeleteErrorList.Add("HasChild", record.HasChild.ToString());
                                        DeleteErrorList.Add("UOM", record.UOM.ToString());
                                        DeleteErrorList.Add("Quantity", record.Quantity.ToString());
                                        DeleteErrorList.Add("BomId", record.BomId);
                                        DeleteErrorList.Add("Action", record.Action);

                                        DeleteErrorDic.Add(DeleteErrorList);
                                        ViewData["deleteErrorDic"] = DeleteErrorDic;
                                    }
                                }
                                catch (Exception)
                                {
                                    DeleteFailureQty++;

                                    Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                    DeleteErrorList.Add("ID", record.ID);
                                    DeleteErrorList.Add("ComponentId", record.ComponentId);
                                    DeleteErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    DeleteErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    DeleteErrorList.Add("HasChild", record.HasChild.ToString());
                                    DeleteErrorList.Add("UOM", record.UOM.ToString());
                                    DeleteErrorList.Add("Quantity", record.Quantity.ToString());
                                    DeleteErrorList.Add("BomId", record.BomId);
                                    DeleteErrorList.Add("Action", record.Action);

                                    DeleteErrorDic.Add(DeleteErrorList);
                                    ViewData["deleteErrorDic"] = DeleteErrorDic;
                                }
                            }
                            else
                            {
                                //错误 忽略

                                Dictionary<string, string> OtherErrorList = new Dictionary<string, string>();

                                OtherErrorList.Add("ID", record.ID);
                                OtherErrorList.Add("ComponentId", record.ComponentId);
                                OtherErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                OtherErrorList.Add("ValidTo", record.ValidTo.ToString());
                                OtherErrorList.Add("HasChild", record.HasChild.ToString());
                                OtherErrorList.Add("UOM", record.UOM.ToString());
                                OtherErrorList.Add("Quantity", record.Quantity.ToString());
                                OtherErrorList.Add("BomId", record.BomId);
                                OtherErrorList.Add("Action", record.Action);

                                OtherErrorDic.Add(OtherErrorList);
                                ViewData["otherErrorDic"] = OtherErrorDic;
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty- DeleteSuccessQty- DeleteFailureQty - ActionNullQty;
                    Dictionary<string, int> Qty = new Dictionary<string, int>();
                    Qty.Add("AllQty", AllQty);
                    Qty.Add("CreateSuccessQty", CreateSuccessQty);
                    Qty.Add("CreateFailureQty", CreateFailureQty);
                    Qty.Add("UpdateSuccessQty", UpdateSuccessQty);
                    Qty.Add("UpdateFailureQty", UpdateFailureQty);
                    Qty.Add("DeleteSuccessQty", DeleteSuccessQty);
                    Qty.Add("DeleteFailureQty", DeleteFailureQty);
                    Qty.Add("ActionNullQty", ActionNullQty);
                    Qty.Add("OtherQty", OtherQty);
                    ViewData["Qty"] = Qty;
                }
                else
                {
                    ViewBag.NotCheckedData = "There are no Data.Please Check Delimiter or Column Name.";
                }
            }
            else
            {
                ViewBag.NotCsv = "Your File is not .Csv File , Please Check FileName.";
            }

            if (ViewBag.NotCsv == null)
            {
                ViewBag.NotCsv = "CSV File is OK.";
            }

            if (ViewBag.NotCheckedData == null)
            {
                ViewBag.NotCheckedData = "Check Data is OK.";
            }

            return View();
        }
    }
}