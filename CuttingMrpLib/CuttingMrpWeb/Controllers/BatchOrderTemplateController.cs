using CuttingMrpWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using CuttingMrpWeb.Models;
using CsvHelper;

namespace CuttingMrpWeb.Controllers
{
    public class BatchOrderTemplateController : Controller
    {
        // GET: BatchOrderTemplate
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            BatchOrderTemplateSearchModel q = new BatchOrderTemplateSearchModel();

            IBatchOrderTemplateService bot = new BatchOrderTemplateService(Settings.Default.db);

            IPagedList<BatchOrderTemplate> bots = bot.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(bots);
        }

        public ActionResult Search([Bind(Include ="OrderNr, PartNr, Type, Remark1")] BatchOrderTemplateSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IBatchOrderTemplateService bot = new BatchOrderTemplateService(Settings.Default.db);

            IPagedList<BatchOrderTemplate> bots = bot.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View("Index", bots);
        }

        public void Export([Bind(Include = "OrderNr, PartNr, Type, Remark1")] BatchOrderTemplateSearchModel q)
        {
            IBatchOrderTemplateService bots = new BatchOrderTemplateService(Settings.Default.db);

            List<BatchOrderTemplate> bot = bots.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "OrderNr", "PartNr", "BatchQuantity", "Type", "Bundle", "CreatedAt", "UpdatedAt", "Operator", "Remark1", "Remark2", "Action" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < bot.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(bot[i].orderNr);
                    ii.Add(bot[i].partNr);
                    ii.Add(bot[i].batchQuantity.ToString());
                    ii.Add(bot[i].type.ToString());
                    ii.Add(bot[i].bundle.ToString());
                    ii.Add(bot[i].createdAt.ToString());
                    ii.Add(bot[i].updatedAt.ToString());
                    ii.Add(bot[i].@operator);
                    ii.Add(bot[i].remark1);
                    ii.Add(bot[i].remark2);
                    ii.Add("");
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "BatchOrderTemplate" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

        public ActionResult ImportBatchOrderTemplateRecord(HttpPostedFileBase batchOrderTemplateFile)
        {
            if(batchOrderTemplateFile == null)
            {
                //TODO:BatchOrderTemplate 上传
                throw new Exception("No file is uploaded to system");
            }

            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(batchOrderTemplateFile.FileName));

            batchOrderTemplateFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<BatchOrderTemplateImportModel> records = new List<BatchOrderTemplateImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<BatchOrderTemplateCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;

                try
                {
                    using (TextReader treader = System.IO.File.OpenText(filename))
                    {
                        CsvReader csvReader = new CsvReader(treader, configuration);
                        records = csvReader.GetRecords<BatchOrderTemplateImportModel>().ToList();

                        Console.Write(records);
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

                    IBatchOrderTemplateService ps = new BatchOrderTemplateService(Settings.Default.db);

                    foreach (BatchOrderTemplateImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            ActionNullQty++;

                            Dictionary<string, string> ActionNullErrorList = new Dictionary<string, string>();

                            ActionNullErrorList.Add("OrderNr", record.OrderNr);
                            ActionNullErrorList.Add("PartNr", record.PartNr);
                            ActionNullErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                            ActionNullErrorList.Add("Type", record.Type.ToString());
                            ActionNullErrorList.Add("Bundle", record.Bundle.ToString());
                            ActionNullErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                            ActionNullErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                            ActionNullErrorList.Add("Operator", record.Operator);
                            ActionNullErrorList.Add("Remark1", record.Remark1);
                            ActionNullErrorList.Add("Remark2", record.Remark2);
                            ActionNullErrorList.Add("Action", record.Action);

                            ActionNullErrorDic.Add(ActionNullErrorList);
                            ViewData["actionNullErrorDic"] = ActionNullErrorDic;
                        }
                        else
                        {
                            //新建
                            BatchOrderTemplate bot = new BatchOrderTemplate()
                            {
                                orderNr = record.OrderNr,
                                partNr = record.PartNr,
                                batchQuantity = record.BatchQuantity,
                                type = record.Type,
                                bundle = record.Bundle,
                                createdAt = record.CreatedAt,
                                updatedAt = record.UpdatedAt,
                                @operator = record.Operator,
                                remark1 = record.Remark1,
                                remark2 = record.Remark2
                            };

                            if (record.Action.Trim().ToLower().Equals("create"))
                            {
                                try
                                {
                                    ps.Create(bot);
                                    CreateSuccessQty++;
                                }
                                catch (Exception)
                                {
                                    CreateFailureQty++;

                                    Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();

                                    CreateErrorList.Add("OrderNr", record.OrderNr);
                                    CreateErrorList.Add("PartNr", record.PartNr);
                                    CreateErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                    CreateErrorList.Add("Type", record.Type.ToString());
                                    CreateErrorList.Add("Bundle", record.Bundle.ToString());
                                    CreateErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                    CreateErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                    CreateErrorList.Add("Operator", record.Operator);
                                    CreateErrorList.Add("Remark1", record.Remark1);
                                    CreateErrorList.Add("Remark2", record.Remark2);
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
                                    bool result = ps.Update(bot);
                                    if (result)
                                    {
                                        UpdateSuccessQty++;
                                    }
                                    else
                                    {
                                        UpdateFailureQty++;
                                        Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();

                                        UpdateErrorList.Add("OrderNr", record.OrderNr);
                                        UpdateErrorList.Add("PartNr", record.PartNr);
                                        UpdateErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                        UpdateErrorList.Add("Type", record.Type.ToString());
                                        UpdateErrorList.Add("Bundle", record.Bundle.ToString());
                                        UpdateErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                        UpdateErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                        UpdateErrorList.Add("Operator", record.Operator);
                                        UpdateErrorList.Add("Remark1", record.Remark1);
                                        UpdateErrorList.Add("Remark2", record.Remark2);
                                        UpdateErrorList.Add("Action", record.Action);

                                        UpdateErrorDic.Add(UpdateErrorList);
                                        ViewData["updateErrorDic"] = UpdateErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    UpdateFailureQty++;

                                    Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();

                                    UpdateErrorList.Add("OrderNr", record.OrderNr);
                                    UpdateErrorList.Add("PartNr", record.PartNr);
                                    UpdateErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                    UpdateErrorList.Add("Type", record.Type.ToString());
                                    UpdateErrorList.Add("Bundle", record.Bundle.ToString());
                                    UpdateErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                    UpdateErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                    UpdateErrorList.Add("Operator", record.Operator);
                                    UpdateErrorList.Add("Remark1", record.Remark1);
                                    UpdateErrorList.Add("Remark2", record.Remark2);
                                    UpdateErrorList.Add("Action", record.Action);

                                    UpdateErrorDic.Add(UpdateErrorList);
                                    ViewData["updateErrorDic"] = UpdateErrorList;
                                }
                            }
                            else if (record.Action.Trim().ToLower().Equals("delete"))
                            {
                                try
                                {
                                    //删除
                                    bool result = ps.Delete(bot);

                                    if (result)
                                    {
                                        DeleteSuccessQty++;
                                    }
                                    else
                                    {
                                        DeleteFailureQty++;

                                        Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                        DeleteErrorList.Add("OrderNr", record.OrderNr);
                                        DeleteErrorList.Add("PartNr", record.PartNr);
                                        DeleteErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                        DeleteErrorList.Add("Type", record.Type.ToString());
                                        DeleteErrorList.Add("Bundle", record.Bundle.ToString());
                                        DeleteErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                        DeleteErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                        DeleteErrorList.Add("Operator", record.Operator);
                                        DeleteErrorList.Add("Remark1", record.Remark1);
                                        DeleteErrorList.Add("Remark2", record.Remark2);
                                        DeleteErrorList.Add("Action", record.Action);

                                        DeleteErrorDic.Add(DeleteErrorList);
                                        ViewData["deleteErrorDic"] = DeleteErrorList;
                                    }
                                }
                                catch (Exception)
                                {
                                    DeleteFailureQty++;

                                    Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                    DeleteErrorList.Add("OrderNr", record.OrderNr);
                                    DeleteErrorList.Add("PartNr", record.PartNr);
                                    DeleteErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                    DeleteErrorList.Add("Type", record.Type.ToString());
                                    DeleteErrorList.Add("Bundle", record.Bundle.ToString());
                                    DeleteErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                    DeleteErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                    DeleteErrorList.Add("Operator", record.Operator);
                                    DeleteErrorList.Add("Remark1", record.Remark1);
                                    DeleteErrorList.Add("Remark2", record.Remark2);
                                    DeleteErrorList.Add("Action", record.Action);

                                    DeleteErrorDic.Add(DeleteErrorList);
                                    ViewData["deleteErrorDic"] = DeleteErrorList;
                                }
                            }
                            else
                            {
                                //错误 忽略

                                Dictionary<string, string> OtherErrorList = new Dictionary<string, string>();

                                OtherErrorList.Add("OrderNr", record.OrderNr);
                                OtherErrorList.Add("PartNr", record.PartNr);
                                OtherErrorList.Add("BatchQuantity", record.BatchQuantity.ToString());
                                OtherErrorList.Add("Type", record.Type.ToString());
                                OtherErrorList.Add("Bundle", record.Bundle.ToString());
                                OtherErrorList.Add("CreatedAt", record.CreatedAt.ToString());
                                OtherErrorList.Add("UpdatedAt", record.UpdatedAt.ToString());
                                OtherErrorList.Add("Operator", record.Operator);
                                OtherErrorList.Add("Remark1", record.Remark1);
                                OtherErrorList.Add("Remark2", record.Remark2);
                                OtherErrorList.Add("Action", record.Action);

                                OtherErrorDic.Add(OtherErrorList);
                                ViewData["otherErrorDic"] = OtherErrorDic;
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty - DeleteSuccessQty - DeleteFailureQty - ActionNullQty;
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
            }else
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