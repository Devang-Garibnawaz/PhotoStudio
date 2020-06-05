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

        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            var tblAlbumAndVideo = db.tblAlbumAndVideoEditingCharges.Include(t => t.tblCustomer);
            return View(tblAlbumAndVideo.ToList());
        }


        [HttpPost]
        public ActionResult getQuotationDetail()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            //int id = Convert.ToInt32(Request.Form["QID"]);
            //return Json(db.tblQuotations.Where(Q => Q.QuotationID == id).Select(Q => new
            //{
            //    CustomerName = Q.tblCustomer.CustomerName,
            //    EventDate = Q.EventDate,
            //    NOCG = Q.NumberOfCinematographers,
            //    NOPG = Q.NumberOfPhotographers,
            //    NOD = Q.NumberOfDrones,
            //    SOLEDS = Q.SizeOfLEDScreen,
            //    NOLEDS = Q.NumberOfLedScreens,
            //    TotalAmount = Q.TotalAmount,
            //    IsDisscount = Q.IsDisccount,
            //    DP = Q.DisscountPercentage,
            //    AAD = Q.AmmountAfterDisscount,
            //    POAP = Q.PercentageOfAdvancePayment,
            //    POPAED = Q.PercentageOfPaymentAtEventDay,
            //    POPAE = Q.PercentageOfPaymentAfterEvent,
            //    IsPass = Q.IsPass

            //}).ToList(), JsonRequestBehavior.AllowGet);
            return View();
        }

        [HttpPost]
        public ActionResult getAlbumAndVideoEditingDetails()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            int id = Convert.ToInt32(Request.Form["AAVEID"]);
            return Json(db.tblAlbumAndVideoEditingCharges.Where(Q => Q.AlbumAndVideoEditingChargesID == id).Select(Q => new
            {
                CustomerName = Q.tblCustomer.CustomerName,
                ALBPG = Q.AlbumPage,
                ALBSZ = Q.AlbumSize,
                ALBType = Q.AlbumType,
                ALBPR = Q.AlbumPrice,
                ALBPDSZ = Q.AlbumPadSize,
                ALBPDPR = Q.AlbumPadPrice,
                ALBLBPR = Q.AlbumLeadherBagPrice,
                PLEDPR = Q.PhotoLEDFramePrice,
                FamilyBannerPR = Q.FamilyBannerPrice,
                SPALB = Q.SpecialAlbum,
                HDVDDUBBPD = Q.HDVideoDubbPendrivePrice

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAAVEC()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View();
        }

        [HttpPost]
        public ActionResult InsertAlbumAndVideoEditingCharge()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    tblAlbumAndVideoEditingCharge AAVEC = new tblAlbumAndVideoEditingCharge();
                    AAVEC.CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    AAVEC.AlbumPage = Convert.ToInt32(Request.Form["AlbumPage"]);
                    AAVEC.AlbumSize = Request.Form["AlbumSize"];
                    AAVEC.AlbumType = Request.Form["AlbumType"];
                    AAVEC.AlbumPrice = Convert.ToDecimal(Request.Form["AlbumPrice"]);
                    AAVEC.AlbumPadSize = Request.Form["AlbumPadSize"];
                    AAVEC.AlbumPadPrice = Convert.ToDecimal(Request.Form["AlbumPadPrice"]);
                    AAVEC.AlbumLeadherBagPrice = Convert.ToDecimal(Request.Form["AlbumLeadherBagPrice"]);
                    AAVEC.PhotoLEDFramePrice = Convert.ToDecimal(Request.Form["PhotoLEDFramePrice"]);
                    AAVEC.FamilyBannerPrice = Convert.ToDecimal(Request.Form["FamilyBannerPrice"]);
                    AAVEC.SpecialAlbum = Convert.ToDecimal(Request.Form["SpecialAlbum"]);
                    AAVEC.HDVideoDubbPendrivePrice = Convert.ToDecimal(Request.Form["HDVideoDubbPendrivePrice"]);
                    AAVEC.Other = Request.Form["Other"];
                    AAVEC.IsDisscount = Request.Form["IsDisscount"] == "true" ? true : false;
                    if (!AAVEC.IsDisscount)
                    {
                        AAVEC.DisscountPercentage = 0;
                        AAVEC.DisscountedAmount = 0;
                        AAVEC.FinalAmount = AAVEC.OriginalAmount;
                    }
                    else
                    {
                        AAVEC.DisscountPercentage = string.IsNullOrEmpty(Request.Form["DisscountPercentage"]) ? 0 : Convert.ToInt32(Request.Form["DisscountPercentage"]);
                        AAVEC.OriginalAmount = string.IsNullOrEmpty(Request.Form["OriginalAmount"]) ? 0 : Convert.ToDecimal(Request.Form["OriginalAmount"]);
                        AAVEC.DisscountedAmount = Convert.ToDecimal(Request.Form["DisscountedAmount"]);
                        AAVEC.FinalAmount = Convert.ToDecimal(Request.Form["FinalAmount"]);
                    }
                    AAVEC.IsPass = false;
                    AAVEC.CreatedDate = DateTime.Now;

                    db.tblAlbumAndVideoEditingCharges.Add(AAVEC);
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

        public ActionResult UpdateAlbumAndVideoEditingCharge(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAlbumAndVideoEditingCharge AAVEC = db.tblAlbumAndVideoEditingCharges.Find(id);
            if (AAVEC == null)
            {
                return HttpNotFound();
            }
            return View(AAVEC);
        }
        [HttpPost]
        public ActionResult UpdateAlbumAndVideoEditingCharge()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    int AAVECID = Convert.ToInt32(Request.Form["AAVECID"]);
                    tblAlbumAndVideoEditingCharge AAVEC = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(AA => AA.AlbumAndVideoEditingChargesID == AAVECID);
                    AAVEC.CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    AAVEC.AlbumPage = Convert.ToInt32(Request.Form["AlbumPage"]);
                    AAVEC.AlbumSize = Request.Form["AlbumSize"];
                    AAVEC.AlbumType = Request.Form["AlbumType"];
                    AAVEC.AlbumPrice = Convert.ToDecimal(Request.Form["AlbumPrice"]);
                    AAVEC.AlbumPadSize = Request.Form["AlbumPadSize"];
                    AAVEC.AlbumPadPrice = Convert.ToDecimal(Request.Form["AlbumPadPrice"]);
                    AAVEC.AlbumLeadherBagPrice = Convert.ToDecimal(Request.Form["AlbumLeadherBagPrice"]);
                    AAVEC.PhotoLEDFramePrice = Convert.ToDecimal(Request.Form["PhotoLEDFramePrice"]);
                    AAVEC.FamilyBannerPrice = Convert.ToDecimal(Request.Form["FamilyBannerPrice"]);
                    AAVEC.SpecialAlbum = Convert.ToDecimal(Request.Form["SpecialAlbum"]);
                    AAVEC.HDVideoDubbPendrivePrice = Convert.ToDecimal(Request.Form["HDVideoDubbPendrivePrice"]);
                    AAVEC.Other = Request.Form["Other"];
                    AAVEC.IsDisscount = Request.Form["IsDisscount"] == "true" ? true : false;
                    if(!AAVEC.IsDisscount)
                    {
                        AAVEC.DisscountPercentage = 0;
                        AAVEC.DisscountedAmount = 0;
                        AAVEC.FinalAmount = AAVEC.OriginalAmount;
                    }
                    else
                    {
                        AAVEC.DisscountPercentage = string.IsNullOrEmpty(Request.Form["DisscountPercentage"]) ? 0 : Convert.ToInt32(Request.Form["DisscountPercentage"]);
                        AAVEC.OriginalAmount = string.IsNullOrEmpty(Request.Form["OriginalAmount"]) ? 0 : Convert.ToDecimal(Request.Form["OriginalAmount"]);
                        AAVEC.DisscountedAmount = Convert.ToDecimal(Request.Form["DisscountedAmount"]);
                        AAVEC.FinalAmount = Convert.ToDecimal(Request.Form["FinalAmount"]);
                    }
                   
                    AAVEC.UpdatedDate = DateTime.Now;

                    db.Entry(AAVEC).State = EntityState.Modified;
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
        public ActionResult DeleteAlbumAndVideoEditing(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblAlbumAndVideoEditingCharge tblAlbumAndVideoEditing = db.tblAlbumAndVideoEditingCharges.Find(id);
                db.tblAlbumAndVideoEditingCharges.Remove(tblAlbumAndVideoEditing);
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
                tblAlbumAndVideoEditingCharge TAAVEC = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(Q => Q.AlbumAndVideoEditingChargesID == id);
                TAAVEC.UpdatedDate = DateTime.Now;
                if (Flag >= 1)
                    TAAVEC.IsPass = true;
                else
                    TAAVEC.IsPass = false;
                db.Entry(TAAVEC).State = EntityState.Modified;
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
