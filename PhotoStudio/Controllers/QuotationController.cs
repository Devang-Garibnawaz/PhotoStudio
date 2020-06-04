using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotoStudio.Models;

namespace PhotoStudio.Controllers
{
    public class QuotationController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Quotation
        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            var tblQuotations = db.tblQuotations.Include(t => t.tblCustomer);
            return View(tblQuotations.ToList());
        }

        [HttpPost]
        public ActionResult getQuotationDetail()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            int id = Convert.ToInt32(Request.Form["QID"]);
            return Json(db.tblQuotations.Where(Q => Q.QuotationID == id).Select(Q => new
            {
                CustomerName = Q.tblCustomer.CustomerName,
                EventDate = Q.EventDate,
                NOCG = Q.NumberOfCinematographers,
                NOPG = Q.NumberOfPhotographers,
                NOD = Q.NumberOfDrones,
                SOLEDS = Q.SizeOfLEDScreen,
                NOLEDS = Q.NumberOfLedScreens,
                TotalAmount = Q.TotalAmount,
                IsDisscount = Q.IsDisccount,
                DP = Q.DisscountPercentage,
                AAD = Q.AmmountAfterDisscount,
                POAP = Q.PercentageOfAdvancePayment,
                POPAED = Q.PercentageOfPaymentAtEventDay,
                POPAE = Q.PercentageOfPaymentAfterEvent,
                IsPass = Q.IsPass

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            ViewBag.CustomerID = new SelectList(db.tblCustomers, "CustomerID", "CustomerName");
            return View();
        }

        [HttpPost]
        public ActionResult InsertQuotation()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    tblQuotation quotation = new tblQuotation();
                    quotation.CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    quotation.EventDate = Convert.ToDateTime(Request.Form["EventDate"]);
                    quotation.NumberOfCinematographers = Convert.ToInt32(Request.Form["NOCG"]);
                    quotation.NumberOfPhotographers = Convert.ToInt32(Request.Form["NOPG"]);
                    quotation.NumberOfDrones = Convert.ToInt32(Request.Form["NOD"]);
                    quotation.SizeOfLEDScreen = Request.Form["SOLEDS"];
                    quotation.NumberOfLedScreens = Convert.ToInt32(Request.Form["NOLEDS"]);
                    quotation.TotalAmount = Convert.ToDecimal(Request.Form["TotalAmount"]);
                    quotation.IsDisccount = Request.Form["IsDisscount"] == "true" ? true : false;
                    quotation.DisscountPercentage = string.IsNullOrEmpty(Request.Form["DP"]) ? 0 : Convert.ToInt32(Request.Form["DP"]);
                    quotation.AmmountAfterDisscount = string.IsNullOrEmpty(Request.Form["AAD"]) ? 0 : Convert.ToDecimal(Request.Form["AAD"]);
                    quotation.PercentageOfAdvancePayment = Convert.ToInt32(Request.Form["POAP"]);
                    quotation.PercentageOfPaymentAtEventDay = Convert.ToInt32(Request.Form["POPAED"]);
                    quotation.PercentageOfPaymentAfterEvent = Convert.ToInt32(Request.Form["POPAE"]);
                    quotation.IsPass = false;
                    quotation.CreatedDate = DateTime.Now;

                    db.tblQuotations.Add(quotation);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Record inserted successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Record is not inserted!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        // GET: Quotation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblQuotation tblQuotation = db.tblQuotations.Find(id);
            if (tblQuotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.tblCustomers, "CustomerID", "CustomerName", tblQuotation.CustomerID);
            return View(tblQuotation);
        }

        // POST: Quotation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult UpdateQuotation()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    int QID = Convert.ToInt32(Request.Form["QID"]);
                    tblQuotation quotation = db.tblQuotations.SingleOrDefault(Q => Q.QuotationID == QID);
                    quotation.CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    quotation.NumberOfCinematographers = Convert.ToInt32(Request.Form["NOCG"]);
                    quotation.NumberOfPhotographers = Convert.ToInt32(Request.Form["NOPG"]);
                    quotation.NumberOfDrones = Convert.ToInt32(Request.Form["NOD"]);
                    quotation.SizeOfLEDScreen = Request.Form["SOLEDS"];
                    quotation.NumberOfLedScreens = Convert.ToInt32(Request.Form["NOLEDS"]);
                    quotation.TotalAmount = Convert.ToDecimal(Request.Form["TotalAmount"]);
                    quotation.IsDisccount = Request.Form["IsDisscount"] == "true" ? true : false;
                    quotation.DisscountPercentage = string.IsNullOrEmpty(Request.Form["DP"]) ? 0 : Convert.ToInt32(Request.Form["DP"]);
                    quotation.AmmountAfterDisscount = string.IsNullOrEmpty(Request.Form["AAD"]) ? 0 : Convert.ToDecimal(Request.Form["AAD"]);
                    quotation.PercentageOfAdvancePayment = Convert.ToInt32(Request.Form["POAP"]);
                    quotation.PercentageOfPaymentAtEventDay = Convert.ToInt32(Request.Form["POPAED"]);
                    quotation.PercentageOfPaymentAfterEvent = Convert.ToInt32(Request.Form["POPAE"]);
                    quotation.IsPass = false;
                    quotation.CreatedDate = DateTime.Now;

                    db.Entry(quotation).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Record updated successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Record is not updated!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteQuotation(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblQuotation tblQuotation = db.tblQuotations.Find(id);
                db.tblQuotations.Remove(tblQuotation);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeIsPassStatus()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                tblQuotation quotation = db.tblQuotations.SingleOrDefault(Q => Q.QuotationID == id);
                quotation.UpdatedDate = DateTime.Now;
                if (Flag >= 1)
                    quotation.IsPass = true;
                else
                    quotation.IsPass = false;
                db.Entry(quotation).State = EntityState.Modified;
                db.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Approved = true, message = "Quotation is Approved" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Approved = true, message = "Quotation is not Approved" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Error = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
