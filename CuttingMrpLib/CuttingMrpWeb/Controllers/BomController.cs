﻿using CsvHelper;
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
                List<string> head = new List<string> { " No.", "ID", "PartNr", "ValidFrom", "ValidTo", "VersionId", "BomDesc, Action"};
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

                    IBomService ps = new BomService(Settings.Default.db);

                    foreach (BomImportModel record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Action))
                        {
                            break;
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
                                catch (Exception e)
                                {
                                    CreateFailureQty++;

                                    ViewBag.MsgCreate = "Some Part Create Failure:";

                                    CreateErrorList.Add(bom.id + e.Message);
                                    ViewData["createError"] = CreateErrorList;
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

                                        ViewBag.MsgUpdate = "Some Part Update Failure:";

                                        UpdateErrorList.Add(bom.id + "Not Exist ID.");
                                        ViewData["updateError"] = UpdateErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    UpdateFailureQty++;

                                    ViewBag.MsgUpdate = "Some Part Update Failure:";

                                    UpdateErrorList.Add(bom.id + e.Message);
                                    ViewData["updateError"] = UpdateErrorList;
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

                                        ViewBag.MsgDelete = "Some Bom Delete Failure:";

                                        DeleteErrorList.Add(bom.id);
                                        ViewData["deleteError"] = DeleteErrorList;
                                    }
                                }
                                catch (Exception e)
                                {
                                    DeleteFailureQty++;

                                    ViewBag.MsgDelete = "Some Part Delete Failure:";

                                    DeleteErrorList.Add("Bom-> ID:" + bom.id + "BomItem 中 存在外键，请先删除BomItem" + e.Message);
                                    ViewData["deleteError"] = DeleteErrorList;
                                }
                            }
                            else
                            {
                                //错误 忽略
                            }
                        }
                    }

                    OtherQty = AllQty - CreateSuccessQty - CreateFailureQty - UpdateSuccessQty - UpdateFailureQty- DeleteSuccessQty- DeleteFailureQty;
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