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
                List<string> head = new List<string> { " No.", "OrderNr", "PartNr", "BatchQuantity", "Type", "Bundle", "CreatedAt", "UpdatedAt", "Operator", "Remarkl", "Remark2", "Action" };
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
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Msg = "Import Failure!" + e;
                }

                List<string> CreateErrorList = new List<string>();
                List<string> UpdateErrorList = new List<string>();
                List<string> DeleteErrorList = new List<string>();


                if (records.Count > 0)
                {
                    int AllQty = records.Count;
                    int CreateSuccessQty = 0;
                    int CreateFailureQty = 0;
                    int UpdateSuccessQty = 0;
                    int UpdateFailureQty = 0;
                    int DeleteSuccessQty = 0;
                    int DeleteFailureQty = 0;
                    int OtherQty = 0;

                    IBatchOrderTemplateService ps = new BatchOrderTemplateService(Settings.Default.db);

                    foreach (BatchOrderTemplateImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            break;
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
                                catch (Exception e)
                                {
                                    CreateFailureQty++;

                                    ViewBag.MsgCreate = "Some Part Create Failure:";

                                    CreateErrorList.Add(bot.orderNr + e.Message);
                                    ViewData["createError"] = CreateErrorList;
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

                                        ViewBag.MsgUpdate = "Some Part Update Failure:";

                                        UpdateErrorList.Add(bot.orderNr + "Not Exist ID.");
                                        ViewData["updateError"] = UpdateErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    UpdateFailureQty++;

                                    ViewBag.MsgUpdate = "Some Part Update Failure:";

                                    UpdateErrorList.Add(bot.orderNr + e.Message);
                                    ViewData["updateError"] = UpdateErrorList;
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

                                        ViewBag.MsgDelete = "Some Bom Delete Failure:";

                                        DeleteErrorList.Add(bot.orderNr);
                                        ViewData["deleteError"] = DeleteErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    DeleteFailureQty++;

                                    ViewBag.MsgDelete = "Some Part Delete Failure:";

                                    DeleteErrorList.Add("BatchOrderTemplate-> OrderNr:" + bot.orderNr + e.Message);
                                    ViewData["deleteError"] = DeleteErrorList;
                                }
                            }
                            else
                            {
                                //错误 忽略
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty - DeleteSuccessQty - DeleteFailureQty;
                    Dictionary<string, int> Qty = new Dictionary<string, int>();
                    Qty.Add("AllQty", AllQty);
                    Qty.Add("CreateSuccessQty", CreateSuccessQty);
                    Qty.Add("CreateFailureQty", CreateFailureQty);
                    Qty.Add("UpdateSuccessQty", UpdateSuccessQty);
                    Qty.Add("UpdateFailureQty", UpdateFailureQty);
                    Qty.Add("DeleteSuccessQty", DeleteSuccessQty);
                    Qty.Add("DeleteFailureQty", DeleteFailureQty);
                    Qty.Add("OtherQty", OtherQty);
                    ViewData["Qty"] = Qty;
                }
                else
                {
                    ViewBag.NoData = "There are no Data.Please Check Delimiter.";
                }
            }

            return View();
        }

    }
}