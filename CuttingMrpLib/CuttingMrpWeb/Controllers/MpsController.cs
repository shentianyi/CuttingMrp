using CuttingMrpWeb.Helpers;
using CuttingMrpWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingMrpLib;
using CuttingMrpWeb.Properties;
using MvcPaging;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text;

namespace CuttingMrpWeb.Controllers
{
    [CustomAuthorize]
    public class MpsController : Controller
    {
        // GET: Mps
        public ActionResult Index(int? page)
        {
            int pageIndex = PagingHelper.GetPageIndex(page);
            MpsSeachModel q = new MpsSeachModel();
            IMpsService ms = new MpsService(Settings.Default.db);

            IPagedList<MP> mps = ms.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetMpStatusDisplayList(null);
            return View(mps);
        }

        public ActionResult Search([Bind(Include = "PartNr, OrderedDateFrom, OrderedDateTo,RequiredDateFrom, RequiredDateTo, Status")] MpsSeachModel q)
        {
            int pageIndex = 0;
            int.TryParse(Request.QueryString.Get("page"), out pageIndex);
            pageIndex = PagingHelper.GetPageIndex(pageIndex);

            IMpsService ms = new MpsService(Settings.Default.db);
            IPagedList<MP> mps = ms.Search(q).ToPagedList(pageIndex, Settings.Default.pageSize);

            ViewBag.Query = q;

            SetMpStatusDisplayList(q.Status);

            return View("Index", mps);
        }

        // GET: Mps/Create
        public ActionResult Create()
        {
            SetMpStatusDisplayList(null);
            return View();
        }

        // POST: Mps/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "id, partnr, orderedDate, requiredDate, quantity, source, sourceDoc,status,unitId")] MP mp)
        {
            try
            {
                IMpsService mps = new MpsService(Settings.Default.db);
                
                mps.Create(mp);

                return RedirectToAction("Index");
            }
            catch
            {
                if (String.IsNullOrWhiteSpace(mp.source))
                {
                    ViewBag.SourceError = "Source 字段是必需的。";
                }

                if (String.IsNullOrWhiteSpace(mp.partnr))
                {
                    ViewBag.Error = "PartNr 字段是必需的。";
                }
                else
                {
                    ViewBag.Error = "PartNr 不存在。";
                }

                SetMpStatusDisplayList(null);
                return View();
            }
        }

        // GET: Mps/Edit/5
        public ActionResult Edit(string id)
        {
            MP mps = GetMpById(id);

            if (mps != null)
            {
                SetMpStatusDisplayList(mps.status);
            }

            return ValidateMps(mps);
        }

        // POST: ProcessOrders/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id, requiredDate, quantity, source, sourceDoc")] MP mp)
        {
            IMpsService mps = new MpsService(Settings.Default.db);
            mps.Update(mp);
            return RedirectToAction("Index");
        }

        // GET: Mps/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return ValidateMps(GetMpById(id));
            }
        }

        // POST: Mps/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            //try
            //{
            IMpsService mps = new MpsService(Settings.Default.db);
            mps.DeleteById(id);
            return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        public void Export([Bind(Include = "PartNr, OrderedDateFrom, OrderedDateTo,RequiredDateFrom, RequiredDateTo, Status")] MpsSeachModel q)
        {
            IMpsService bs = new MpsService(Settings.Default.db);

            List<MP> mps = bs.Search(q).ToList();

            ViewBag.Query = q;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
            {
                List<string> head = new List<string> { " No.", "ID", "PartNr", "OrderedDate", "RequiredDate", "Quantity", "Source", "SourceDoc", "Status", "UnitId", "Action" };
                sw.WriteLine(string.Join(Settings.Default.csvDelimiter, head));
                for (var i = 0; i < mps.Count; i++)
                {
                    List<string> ii = new List<string>();
                    ii.Add((i + 1).ToString());
                    ii.Add(mps[i].id.ToString());
                    ii.Add(mps[i].partnr);
                    ii.Add(mps[i].orderedDate.ToString());
                    ii.Add(mps[i].requiredDate.ToString());
                    ii.Add(mps[i].quantity.ToString());
                    ii.Add(mps[i].source);
                    ii.Add(mps[i].sourceDoc);
                    ii.Add(mps[i].status.ToString());
                    ii.Add(mps[i].unitId);
                    ii.Add("");
                    sw.WriteLine(string.Join(Settings.Default.csvDelimiter, ii.ToArray()));
                }
                //sw.WriteLine(max);
            }
            var filename = "Mps" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var contenttype = "text/csv";
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contenttype;
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }

        public ActionResult ImportMpsRecord(HttpPostedFileBase mpsFile)
        {
            if (mpsFile == null)
            {
                //TODO: Mps Import 优化上传失败界面
                throw new Exception("No file is uploaded to system");
            }

            var appData = Server.MapPath("~/TmpFile/");
            var filename = Path.Combine(appData,
                DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(mpsFile.FileName));

            mpsFile.SaveAs(filename);
            string ex = Path.GetExtension(filename);

            List<MpsImportModel> records = new List<MpsImportModel>();

            if (ex.Equals(".csv"))
            {
                CsvConfiguration configuration = new CsvConfiguration();
                configuration.Delimiter = Settings.Default.csvDelimiter;
                configuration.HasHeaderRecord = true;
                configuration.SkipEmptyRecords = true;
                configuration.RegisterClassMap<MpsCsvModelMap>();
                configuration.TrimHeaders = true;
                configuration.TrimFields = true;

                try
                {
                    using (TextReader treader = System.IO.File.OpenText(filename))
                    {
                        CsvReader csvReader = new CsvReader(treader, configuration);
                        records = csvReader.GetRecords<MpsImportModel>().ToList();

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

                    IMpsService ps = new MpsService(Settings.Default.db);

                    foreach (MpsImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            ActionNullQty++;

                            Dictionary<string, string> ActionNullErrorList = new Dictionary<string, string>();

                            ActionNullErrorList.Add("ID", record.ID);
                            ActionNullErrorList.Add("PartNr", record.PartNr);
                            ActionNullErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                            ActionNullErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                            ActionNullErrorList.Add("Quantity", record.Quantity.ToString());
                            ActionNullErrorList.Add("Source", record.Source);
                            ActionNullErrorList.Add("SourceDoc", record.SourceDoc);
                            ActionNullErrorList.Add("Status", record.Status.ToString());
                            ActionNullErrorList.Add("UnitId", record.UnitId);
                            ActionNullErrorList.Add("Action", record.Action);

                            ActionNullErrorDic.Add(ActionNullErrorList);
                            ViewData["actionNullErrorDic"] = ActionNullErrorDic;
                        }
                        else
                        {
                            MP mps = new MP()
                            {
                                id = Convert.ToInt32(record.ID),
                                partnr = record.PartNr,
                                orderedDate= record.OrderedDate,
                                requiredDate = record.RequiredDate,
                                quantity = record.Quantity,
                                source = record.Source,
                                sourceDoc = record.SourceDoc,
                                status = record.Status,
                                unitId = record.UnitId
                            };

                            if (record.Action.Trim().ToLower().Equals("create"))
                            {
                                //新建
                                try
                                {
                                    ps.Create(mps);
                                    CreateSuccessQty++;
                                }
                                catch (Exception)
                                {
                                    CreateFailureQty++;

                                    Dictionary<string, string> CreateErrorList = new Dictionary<string, string>();

                                    CreateErrorList.Add("ID", record.ID);
                                    CreateErrorList.Add("PartNr", record.PartNr);
                                    CreateErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                    CreateErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                    CreateErrorList.Add("Quantity", record.Quantity.ToString());
                                    CreateErrorList.Add("Source", record.Source);
                                    CreateErrorList.Add("SourceDoc", record.SourceDoc);
                                    CreateErrorList.Add("Status", record.Status.ToString());
                                    CreateErrorList.Add("UnitId", record.UnitId);
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
                                    bool result = ps.Update(mps);
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
                                        UpdateErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                        UpdateErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                        UpdateErrorList.Add("Quantity", record.Quantity.ToString());
                                        UpdateErrorList.Add("Source", record.Source);
                                        UpdateErrorList.Add("SourceDoc", record.SourceDoc);
                                        UpdateErrorList.Add("Status", record.Status.ToString());
                                        UpdateErrorList.Add("UnitId", record.UnitId);
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
                                    UpdateErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                    UpdateErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                    UpdateErrorList.Add("Quantity", record.Quantity.ToString());
                                    UpdateErrorList.Add("Source", record.Source);
                                    UpdateErrorList.Add("SourceDoc", record.SourceDoc);
                                    UpdateErrorList.Add("Status", record.Status.ToString());
                                    UpdateErrorList.Add("UnitId", record.UnitId);
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
                                    bool result = ps.DeleteById(mps.id.ToString());

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
                                        DeleteErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                        DeleteErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                        DeleteErrorList.Add("Quantity", record.Quantity.ToString());
                                        DeleteErrorList.Add("Source", record.Source);
                                        DeleteErrorList.Add("SourceDoc", record.SourceDoc);
                                        DeleteErrorList.Add("Status", record.Status.ToString());
                                        DeleteErrorList.Add("UnitId", record.UnitId);
                                        DeleteErrorList.Add("Action", record.Action);

                                        DeleteErrorDic.Add(DeleteErrorList);
                                        ViewData["deleteErrorDic"] = DeleteErrorDic;
                                    }
                                }
                                catch (Exception e)
                                {
                                    DeleteFailureQty++;
                                    Dictionary<string, string> DeleteErrorList = new Dictionary<string, string>();

                                    DeleteErrorList.Add("ID", record.ID);
                                    DeleteErrorList.Add("PartNr", record.PartNr);
                                    DeleteErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                    DeleteErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                    DeleteErrorList.Add("Quantity", record.Quantity.ToString());
                                    DeleteErrorList.Add("Source", record.Source);
                                    DeleteErrorList.Add("SourceDoc", record.SourceDoc);
                                    DeleteErrorList.Add("Status", record.Status.ToString());
                                    DeleteErrorList.Add("UnitId", record.UnitId);
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
                                OtherErrorList.Add("OrderedDate", record.OrderedDate.ToString());
                                OtherErrorList.Add("RequiredDate", record.RequiredDate.ToString());
                                OtherErrorList.Add("Quantity", record.Quantity.ToString());
                                OtherErrorList.Add("Source", record.Source);
                                OtherErrorList.Add("SourceDoc", record.SourceDoc);
                                OtherErrorList.Add("Status", record.Status.ToString());
                                OtherErrorList.Add("UnitId", record.UnitId);
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

        private ActionResult ValidateMps(MP mps)
        {
            if (mps == null)
            {
                return HttpNotFound();
            }

            return View(mps);
        }

        private MP GetMpById(string id)
        {
            IMpsService ps = new MpsService(Settings.Default.db);
            MP mp = ps.FindById(id);
            return mp;
        }

        private void SetMpStatusDisplayList(int? type, bool allowBlank = true)
        {
            List<EnumItem> item = EnumUtility.GetList(typeof(MPSStatus));

            List<SelectListItem> select = new List<SelectListItem>();

            if (allowBlank)
            {
                select.Add(new SelectListItem { Text = "", Value = "" });
            }

            foreach(var it in item)
            {
                if(type.HasValue&& type.ToString().Equals(it.Value))
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = true });
                }else
                {
                    select.Add(new SelectListItem { Text = it.Text, Value = it.Value.ToString(), Selected = false });
                }
            }

            ViewData["statusList"] = select;
        }
    }
}