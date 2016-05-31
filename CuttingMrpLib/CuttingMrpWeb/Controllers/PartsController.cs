using CuttingMrpLib;
using CuttingMrpWeb.Models;
using CuttingMrpWeb.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuttingMrpWeb.Controllers
{
    public class PartsController : Controller
    {
        // GET: Parts
        public ActionResult Index()
        {
            return View();
        }

        // GET: Parts/Details/5
    
        [HttpGet]
        public JsonResult Details(string id)
        {
            IPartService ps = new PartService(Settings.Default.db);
            Part part = ps.FindById(id);
            PartViewModel pv = new PartViewModel()
            {
                partNr = part.partNr,
                partTypeDisplay = part.partTypeDisplay,
                partDesc = part.partDesc,
                moq = part.moq,
                spq = part.spq,
                kanbanNr = part.kanbanNrs
            };
            return Json(pv, JsonRequestBehavior.AllowGet);
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
    }
}
