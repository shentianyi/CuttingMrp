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
    public class PartsController : Controller
    {
        // GET: Parts
        [CustomAuthorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            PartSearchModel q = new PartSearchModel();
            IPartService ps = new PartService(Settings.Default.db);

            IPagedList<Part> parts = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetPartTypeList(null);

            return View(parts);
        }

        // GET: Parts/Details/5

        [HttpGet]
        public JsonResult Details(string id)
        {
            IPartService ps = new PartService(Settings.Default.db);
            Part part = ps.FindById(id);
            PartViewModel pv = null;
            if (part != null)
            {
                pv = new PartViewModel()
                {
                    partNr = part.partNr,
                    partTypeDisplay = part.partTypeDisplay,
                    partDesc = part.partDesc,
                    moq = part.moq,
                    spq = part.spq,
                    kanbanNr = part.kanbanNrs,
                    position=part.kanbanPosition
                };
            }
            return Json(pv, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Fuzzies(string id) {
            IPartService ps = new PartService(Settings.Default.db);
            List<Part> parts = ps.FuzzyById(id);

           List< PartViewModel> pvs = new List<PartViewModel> ();
            foreach (var part in parts)
            {
                pvs.Add(new PartViewModel()
                {
                    partNr = part.partNr
                });
            }
            return Json(pvs, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Parents(string id) {
            IPartService ps = new PartService(Settings.Default.db);
            List<Part> parts = ps.GetParentParts(id);
            List<PartViewModel> pvs = new List<PartViewModel>();
            foreach (var part in parts)
            {
                pvs.Add(new PartViewModel()
                {
                    partNr = part.partNr,
                    partTypeDisplay = part.partTypeDisplay,
                    partDesc = part.partDesc,
                    moq = part.moq,
                    spq = part.spq
                });
            }
            return Json(pvs, JsonRequestBehavior.AllowGet);
        }
        // GET: Parts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parts/Create
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

        // GET: Parts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Parts/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: Parts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Parts/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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

        public ActionResult Search([Bind(Include = "PartNr, PartType")] PartSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IPartService ps = new PartService(Settings.Default.db);

            IPagedList<Part> parts = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetPartTypeList(q.PartType);
            return View("Index", parts);
        }

        public ActionResult ImportPartRecord(HttpPostedFileBase partFile)
        {
            if(partFile == null)
            {
                //TODO: Parts 上传， 如果没有路径，在此处进行友好处理
                throw new Exception("No file is uploaded to system");
            }

            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(partFile.FileName));

            partFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<PartImportModel> records = new List<PartImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<PartCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;

                try
                {
                    using (TextReader treader = System.IO.File.OpenText(filename))
                    {
                        CsvReader csvReader = new CsvReader(treader, configuration);
                        records = csvReader.GetRecords<PartImportModel>().ToList();
                    }
                }
                catch (Exception e)
                {
                    //ViewBag.TextExpMsg = "<-------------Read Csv File Exception!,Please Check.------------->" + e;
                    ViewBag.TextExpMsg = "<-------------读取CSV文件异常，请查看原因：------------->" + e;
                }

                List<Dictionary<string, string>> CreateErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> UpdateErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> ActionNullErrorDic = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> OtherErrorDic = new List<Dictionary<string, string>>();

                if (records.Count > 0)
                {
                    IPartService ps = new PartService(Settings.Default.db);

                    int AllQty = records.Count;
                    int CreateSuccessQty = 0;
                    int CreateFailureQty = 0;
                    int UpdateSuccessQty = 0;
                    int UpdateFailureQty = 0;
                    int ActionNullQty = 0;
                    int OtherQty = 0;

                    foreach (PartImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            ActionNullQty++;

                            Dictionary<string, string> ActionNullErrorList = new Dictionary<string, string>();
                            ActionNullErrorList.Add("partNr", record.PartNr);
                            ActionNullErrorList.Add("partType", record.PartType.ToString());
                            ActionNullErrorList.Add("partDesc", record.PartDesc);
                            ActionNullErrorList.Add("partStatus", record.PartStatus.ToString());
                            ActionNullErrorList.Add("MOQ", record.MOQ.ToString());
                            ActionNullErrorList.Add("SPQ", record.SPQ.ToString());
                            ActionNullErrorList.Add("Action", record.Action);

                            ActionNullErrorDic.Add(ActionNullErrorList);
                            ViewData["actionNullErrorDic"] = ActionNullErrorDic;
                        }
                        else
                        {
                            //新建
                            Part part = new Part()
                            {
                                partNr = record.PartNr,
                                partType = record.PartType,
                                partDesc = record.PartDesc,
                                partStatus = record.PartStatus,
                                moq = record.MOQ,
                                spq = record.SPQ
                            };

                            if (record.Action.Trim().ToLower().Equals("create"))
                            {
                                try
                                {
                                   bool result= ps.Create(part);

                                    if (result)
                                    {
                                        CreateSuccessQty++;
                                    }
                                    else
                                    {
                                        CreateFailureQty++;

                                        Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();
                                        CreateErrorList.Add("partNr", record.PartNr);
                                        CreateErrorList.Add("partType", record.PartType.ToString());
                                        CreateErrorList.Add("partDesc", record.PartDesc);
                                        CreateErrorList.Add("partStatus", record.PartStatus.ToString());
                                        CreateErrorList.Add("MOQ", record.MOQ.ToString());
                                        CreateErrorList.Add("SPQ", record.SPQ.ToString());
                                        CreateErrorList.Add("Action", record.Action);

                                        CreateErrorDic.Add(CreateErrorList);
                                        ViewData["createErrorDic"] = CreateErrorDic;
                                    }
                                }
                                catch (Exception)
                                {
                                    CreateFailureQty++;
                                    //ViewBag.CreateExpMsg = "<-------------Create Part Exception!,Maybe partNr is Exist,Please Check.------------->";
                                    ViewBag.CreateExpMsg = "<-------------创建零件异常，可能PartNr列已经存在，请仔细检查。------------->";
                                }
                            }
                            else if (record.Action.Trim().ToLower().Equals("update"))
                            {
                                //更新
                                try
                                {
                                    bool result= ps.Update(part);
                                    if (result)
                                    {
                                        UpdateSuccessQty++;
                                    }
                                    else
                                    {
                                        UpdateFailureQty++;

                                        Dictionary<string, string> UpdateErrorList = new Dictionary<string, string>();
                                        UpdateErrorList.Add("partNr", record.PartNr);
                                        UpdateErrorList.Add("partType", record.PartType.ToString());
                                        UpdateErrorList.Add("partDesc", record.PartDesc);
                                        UpdateErrorList.Add("partStatus", record.PartStatus.ToString());
                                        UpdateErrorList.Add("MOQ", record.MOQ.ToString());
                                        UpdateErrorList.Add("SPQ", record.SPQ.ToString());
                                        UpdateErrorList.Add("Action", record.Action);

                                        UpdateErrorDic.Add(UpdateErrorList);
                                        ViewData["updateErrorDic"] = UpdateErrorDic;
                                    }
                                }
                                catch (Exception e)
                                {
                                    UpdateFailureQty++;

                                    //ViewBag.UpdateExpMsg = "<-------------Update Part Exception!,Please Check.------------->" + e;
                                    ViewBag.UpdateExpMsg = "<-------------更新零件异常，可能ID或PartNr不存在，请仔细检查。------------->" + e;

                                }
                            }
                            else
                            {
                                //错误 不做任何操作，只需要记录
                                Dictionary<string, string> OtherErrorList = new Dictionary<string, string>();
                                OtherErrorList.Add("partNr", record.PartNr);
                                OtherErrorList.Add("partType", record.PartType.ToString());
                                OtherErrorList.Add("partDesc", record.PartDesc);
                                OtherErrorList.Add("partStatus", record.PartStatus.ToString());
                                OtherErrorList.Add("MOQ", record.MOQ.ToString());
                                OtherErrorList.Add("SPQ", record.SPQ.ToString());
                                OtherErrorList.Add("Action", record.Action);

                                OtherErrorDic.Add(OtherErrorList);
                                ViewData["otherErrorDic"] = OtherErrorDic;
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty - ActionNullQty;

                    Dictionary<string, int> Qty = new Dictionary<string, int>();

                    Qty.Add("AllQty", AllQty);
                    Qty.Add("CreateSuccessQty", CreateSuccessQty);
                    Qty.Add("CreateFailureQty", CreateFailureQty);
                    Qty.Add("UpdateSuccessQty", UpdateSuccessQty);
                    Qty.Add("UpdateFailureQty", UpdateFailureQty);
                    Qty.Add("ActionNullQty", ActionNullQty);
                    Qty.Add("OtherQty", OtherQty);

                    ViewData["Qty"] = Qty;
                }
                else
                {
                    //ViewBag.NotCheckedData = "No Data Checked. Please Check Delimiter or Column Name.";
                    ViewBag.NotCheckedData = "没有检测到数据。请检查分隔符和列名。";
                }
            }else
            {
                //ViewBag.NotCsv = "Your File is not .Csv File, Please Check FileName.";
                ViewBag.NotCsv = "你上传的文件不是.CSV格式。请检查文件名。";
            }

            if (ViewBag.NotCsv ==null)
            {
                //ViewBag.NotCsv = "CSV File is OK.";
                ViewBag.NotCsv = "上传CSV文件正确!";
            }

            if (ViewBag.NotCheckedData==null)
            {
                //ViewBag.NotCheckedData = "Check Data is OK.";
                ViewBag.NotCheckedData = "检查数据完成!";
            }

            return View();
        }

        public void Export([Bind(Include = "PartNr, PartType")] PartSearchModel q)
        {
            IPartService ps = new PartService(Settings.Default.db);

            List<Part> parts = ps.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "PartNr", "PartType", "PartDesc","PartStatus","MOQ",
                    "SPQ", "Action"};
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < parts.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(parts[i].partNr);
                    ii.Add(parts[i].partType.ToString());
                    ii.Add(parts[i].partDesc);
                    ii.Add(parts[i].partStatus.ToString());
                    ii.Add(parts[i].moq.ToString());
                    ii.Add(parts[i].spq.ToString());
                    ii.Add("");
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Parts" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

        private void SetPartTypeList(int? status, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(PartType));

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
            ViewData["partTypeList"] = select;
        }
    }
}
