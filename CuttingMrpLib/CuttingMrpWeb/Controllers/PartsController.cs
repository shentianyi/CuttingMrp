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
                // TODO: Add insert logic here

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
                // TODO: Add update logic here

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
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search([Bind(Include = "PartNr")] PartSearchModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IPartService ps = new PartService(Settings.Default.db);

            IPagedList<Part> parts = ps.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            return View("Index", parts);
        }

        public ActionResult ImportPartRecord(HttpPostedFileBase partFile)
        {
            if(partFile == null)
            {
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
                    ViewBag.Msg = "Import Failure!" + e;
                }

                List<string> CreateErrorList = new List<string>();
                List<string> UpdateErrorList = new List<string>();
                

                if (records.Count > 0)
                {
                    int AllQty = records.Count;
                    int CreateSuccessQty = 0;
                    int CreateFailureQty = 0;
                    int UpdateSuccessQty = 0;
                    int UpdateFailureQty = 0;
                    int OtherQty = 0;

                    IPartService ps = new PartService(Settings.Default.db);

                    foreach (PartImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            break;
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

                                        ViewBag.MsgCreate = "Some Part Create Failure:";

                                        CreateErrorList.Add(part.partNr);
                                        ViewData["createError"] = CreateErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    ViewBag.MsgCreate = "Create Failure!" + e;
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
                                       
                                        ViewBag.MsgUpdate = "Some Part Update Failure:";

                                        UpdateErrorList.Add(part.partNr);
                                        ViewData["updateError"] = UpdateErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    ViewBag.MsgUpdate = "Update Failure!" + e;
                                }   
                            }
                            else if (record.Action.Trim().ToLower().Equals("delete"))
                            {
                                //删除  忽略
                            }
                            else
                            {
                                //错误 忽略
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty;
                    Dictionary<string, int> Qty = new Dictionary<string, int>();
                    Qty.Add("AllQty", AllQty);
                    Qty.Add("CreateSuccessQty", CreateSuccessQty);
                    Qty.Add("CreateFailureQty", CreateFailureQty);
                    Qty.Add("UpdateSuccessQty", UpdateSuccessQty);
                    Qty.Add("UpdateFailureQty", UpdateFailureQty);
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

        public void Export([Bind(Include = "PartNr")] PartSearchModel q)
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
