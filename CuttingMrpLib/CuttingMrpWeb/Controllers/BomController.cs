using CsvHelper;
using CsvHelper.Configuration;
using CuttingMrpLib;
using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CuttingMrpWeb.Controllers
{
    public class BomController : Controller
    {
        // GET: Bom
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);

            BomSearchModel q = new BomSearchModel();

            IBomService bs = new BomService(Settings.Default.db);

            IPagedList<BOM> boms = bs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View(boms);
        }

        public ActionResult Search([Bind(Include ="ID, PartNr, VFFrom, VFTo, VTFrom, VTTo")] BomSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IBomService bs = new BomService(Settings.Default.db);

            IPagedList<BOM> boms= bs.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View("Index", boms);
        }

        public void Export([Bind(Include = "ID, PartNr, VFFrom, VFTo, VTFrom, VTTo")] BomSearchModel q)
        {
            IBomService bs = new BomService(Settings.Default.db);

            List<BOM> boms = bs.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "ID", "PartNr", "ValidFrom", "ValidTo", "VersionId", "BomDesc", "Action"};
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < boms.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(boms[i].id.ToString());
                    ii.Add(boms[i].partNr);
                    ii.Add(boms[i].validFrom.ToString());
                    ii.Add(boms[i].validTo.ToString());
                    ii.Add(boms[i].versionId);
                    ii.Add(boms[i].bomDesc);
                    ii.Add("");
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Bom" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

        public ActionResult ImportBomRecord(HttpPostedFileBase bomFile)
        {
            if (bomFile == null)
            {
                //TODO: Bom Import 优化上传失败界面
                throw new Exception("No file is uploaded to system");
            }

            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(bomFile.FileName));

            bomFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<BomImportModel> records = new List<BomImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<BomCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;

                try
                {
                    using (TextReader treader = System.IO.File.OpenText(filename))
                    {
                        CsvReader csvReader = new CsvReader(treader, configuration);
                        records = csvReader.GetRecords<BomImportModel>().ToList();
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

                    IBomService ps = new BomService(Settings.Default.db);

                    foreach (BomImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            ActionNullQty++;

                            Dictionary<string, string> ActionNullErrorList = new Dictionary<string, string>();

                            ActionNullErrorList.Add("ID", record.ID);
                            ActionNullErrorList.Add("PartNr", record.PartNr);
                            ActionNullErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                            ActionNullErrorList.Add("ValidTo", record.ValidTo.ToString());
                            ActionNullErrorList.Add("VersionId", record.VersionId);
                            ActionNullErrorList.Add("BomDesc", record.BomDesc);
                            ActionNullErrorList.Add("Action", record.Action);

                            ActionNullErrorDic.Add(ActionNullErrorList);
                            ViewData["actionNullErrorDic"] = ActionNullErrorDic;
                        }
                        else
                        {
                            //新建
                            BOM bom = new BOM()
                            {
                                id = record.ID,
                                partNr = record.PartNr,
                                validFrom = record.ValidFrom,
                                validTo = record.ValidTo,
                                versionId = record.VersionId,
                                bomDesc = record.BomDesc
                            };

                            if (record.Action.Trim().ToLower().Equals("create"))
                            {
                                try
                                {
                                    ps.Create(bom);
                                    CreateSuccessQty++;
                                }
                                catch (Exception)
                                {
                                    CreateFailureQty++;

                                    Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();

                                    CreateErrorList.Add("ID", record.ID);
                                    CreateErrorList.Add("PartNr", record.PartNr);
                                    CreateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    CreateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    CreateErrorList.Add("VersionId", record.VersionId);
                                    CreateErrorList.Add("BomDesc", record.BomDesc);
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
                                    bool result = ps.Update(bom);
                                    if (result)
                                    {
                                        UpdateSuccessQty++;
                                    }
                                    else
                                    {
                                        UpdateFailureQty++;

                                        Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();

                                        UpdateErrorList.Add("ID", record.ID);
                                        UpdateErrorList.Add("PartNr", record.PartNr);
                                        UpdateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                        UpdateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                        UpdateErrorList.Add("VersionId", record.VersionId);
                                        UpdateErrorList.Add("BomDesc", record.BomDesc);
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
                                    UpdateErrorList.Add("PartNr", record.PartNr);
                                    UpdateErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    UpdateErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    UpdateErrorList.Add("VersionId", record.VersionId);
                                    UpdateErrorList.Add("BomDesc", record.BomDesc);
                                    UpdateErrorList.Add("Action", record.Action);

                                    UpdateErrorDic.Add(UpdateErrorList);
                                    ViewData["updateErrorDic"] = UpdateErrorDic;
                                }
                            }
                            else if (record.Action.Trim().ToLower().Equals("delete"))
                            {
                                try
                                {
                                    //删除
                                    bool result = ps.Delete(bom);

                                    if (result)
                                    {
                                        DeleteSuccessQty++;
                                    }
                                    else
                                    {
                                        DeleteFailureQty++;

                                        Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                        DeleteErrorList.Add("ID", record.ID);
                                        DeleteErrorList.Add("PartNr", record.PartNr);
                                        DeleteErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                        DeleteErrorList.Add("ValidTo", record.ValidTo.ToString());
                                        DeleteErrorList.Add("VersionId", record.VersionId);
                                        DeleteErrorList.Add("BomDesc", record.BomDesc);
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
                                    DeleteErrorList.Add("PartNr", record.PartNr);
                                    DeleteErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                    DeleteErrorList.Add("ValidTo", record.ValidTo.ToString());
                                    DeleteErrorList.Add("VersionId", record.VersionId);
                                    DeleteErrorList.Add("BomDesc", record.BomDesc);
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
                                OtherErrorList.Add("PartNr", record.PartNr);
                                OtherErrorList.Add("ValidFrom", record.ValidFrom.ToString());
                                OtherErrorList.Add("ValidTo", record.ValidTo.ToString());
                                OtherErrorList.Add("VersionId", record.VersionId);
                                OtherErrorList.Add("BomDesc", record.BomDesc);
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
                    ViewBag.NotCheckedData = "No Data Checked. Please Check Delimiter or Column Name.";
                }
            }
            else
            {
                ViewBag.NotCsv = "Your File is not .Csv File, Please Check FileName.";
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