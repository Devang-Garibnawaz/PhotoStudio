using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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

        public ActionResult AddEmployees()
        {
            return View();
        }

        public ActionResult AddQuotation()
        {
            return View();
        }
        public ActionResult ManageQuotationDetails(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAlbumAndVideoEditingCharge AAVEC = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(A => A.AlbumAndVideoEditingChargesID == id);
            ViewBag.AAVECID = AAVEC.AlbumAndVideoEditingChargesID;
            ViewBag.IsDisscount = AAVEC.IsDisscount;
            ViewBag.IsPass = AAVEC.IsPass == true ? true : false;
            return View(db.tblQuotations.Where(Q => Q.AlbumAndVideoEditingChargesID == id));
        }

        [HttpPost]
        public ActionResult getQuotationDetail()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            int id = Convert.ToInt32(Request.Form["CustomerID"]);
            return Json(db.tblQuotations.Where(Q => Q.tblAlbumAndVideoEditingCharge.tblCustomer.CustomerID  == id).Select(Q => new
            {
                QuotationID = Q.QuotationID,
                EventDate = Q.EventDate,
                Function = Q.FunctionName,
                CandidCinemato = Q.CandidCinematographers,
                RegularCinemato  = Q.RegularCinematographers,
                CandidPhoto = Q.CandidPhotographer,
                RegularPhoto = Q.RegularPhotographer,
                DSLR = Q.DSLR,
                Drones = Q.NumberOfDrones,
                LED = Q.NumberOfLedScreens,
                Other = Q.Others,
                TotalAmount = Q.TotalAmount

            }).ToList(), JsonRequestBehavior.AllowGet);
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
                ALBPR = Q.AlbumPrice,
                ALBPDSZ = Q.AlbumPadSize,
                ALBPDPR = Q.AlbumPadPrice,
                PLEDPR = Q.PhotoLEDFramePrice,
                SPALB = Q.SpecialAlbum,
                HDVDDUBBPD = Q.HDVideoDubbPendrivePrice,
                OriginalAmount = Q.OriginalAmount,
                DissPer=Q.DiscountPercentage,
                DissAmt=Q.DiscountedAmount,
                FinalAmt=Q.FinalAmount

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintQuotation(int? id)
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
            tblAlbumAndVideoEditingCharge AAVECDetails = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(A => A.AlbumAndVideoEditingChargesID == id);
            ViewBag.CustomerName = AAVECDetails.tblCustomer.CustomerName;
            ViewBag.AAVECID = AAVECDetails.AlbumAndVideoEditingChargesID;
            return View(db.tblQuotations.Where(Q => Q.AlbumAndVideoEditingChargesID == id));
        }

        [HttpPost]
        public ActionResult getQuotationDetails()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            int id = Convert.ToInt32(Request.Form["QID"]);
            return Json(db.tblQuotations.Where(Q => Q.QuotationID == id).Select(Q => new
            {
                CCG = Q.CandidCinematographers,
                RCG = Q.RegularCinematographers,
                CPG = Q.CandidPhotographer,
                RPG = Q.RegularPhotographer,
                DSLR = Q.DSLR,
                Drones = Q.NumberOfDrones,
                NOLEDS = Q.NumberOfLedScreens
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
            long LastAAVECID = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    tblAlbumAndVideoEditingCharge AAVEC = new tblAlbumAndVideoEditingCharge();
                    AAVEC.CustomerID = Convert.ToInt64(Request.Form["CustomerID"]);
                    if(IsQuotationIsExist(Convert.ToInt64(AAVEC.CustomerID)))
                        return Json(new { success = false, message = "Quotation of this user is already created!" }, JsonRequestBehavior.AllowGet);

                    AAVEC.AlbumPage = Convert.ToInt32(Request.Form["AlbumPage"]);
                    AAVEC.AlbumSize = Request.Form["AlbumSize"];
                    AAVEC.AlbumPrice = Convert.ToDecimal(Request.Form["AlbumPrice"]);
                    AAVEC.AlbumPadSize = Request.Form["AlbumPadSize"];
                    AAVEC.AlbumPadPrice = Convert.ToDecimal(Request.Form["AlbumPadPrice"]);
                    
                    AAVEC.PhotoLEDFramePrice = Convert.ToDecimal(Request.Form["PhotoLEDFramePrice"]);
                    
                    AAVEC.SpecialAlbum = Convert.ToDecimal(Request.Form["SpecialAlbum"]);
                    AAVEC.HDVideoDubbPendrivePrice = Convert.ToDecimal(Request.Form["HDVideoDubbPendrivePrice"]);
                    AAVEC.Other = Request.Form["Other"];
                    AAVEC.IsDisscount = Request.Form["IsDisscount"] == "true" ? true : false;
                    AAVEC.OriginalAmount = string.IsNullOrEmpty(Request.Form["OriginalAmount"]) ? 0 : Convert.ToDecimal(Request.Form["OriginalAmount"]);

                    if (AAVEC.IsDisscount != true)
                    {
                        AAVEC.DiscountPercentage = 0;
                        AAVEC.DiscountedAmount = 0;
                        AAVEC.FinalAmount = AAVEC.OriginalAmount;
                    }
                    else
                    {
                        AAVEC.DiscountPercentage = string.IsNullOrEmpty(Request.Form["DiscountPercentage"]) ? Convert.ToInt64(0) : Convert.ToInt64(Request.Form["DiscountPercentage"]);
                        AAVEC.DiscountedAmount = Convert.ToDecimal(Request.Form["DiscountedAmount"]);
                        AAVEC.FinalAmount = Convert.ToDecimal(Request.Form["FinalAmount"]);
                    }
                    AAVEC.IsPass = false;
                    AAVEC.CreatedDate = DateTime.Now;

                    db.tblAlbumAndVideoEditingCharges.Add(AAVEC);
                    db.SaveChanges();
                    LastAAVECID = db.tblAlbumAndVideoEditingCharges.Max(item => item.AlbumAndVideoEditingChargesID); ;
                    return Json(new { success = true, message = "Record inserted successfully",lastAAVECID = LastAAVECID }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Record is not inserted!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

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
                    int AAVECID = Convert.ToInt32(Request.Form["AAVECID"]);
                    tblAlbumAndVideoEditingCharge AAVEC = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(A => A.AlbumAndVideoEditingChargesID == AAVECID);
                    tblQuotation quotation = new tblQuotation();
                    quotation.AlbumAndVideoEditingChargesID = AAVECID;
                    quotation.EventDate = Convert.ToDateTime(Request.Form["EventDate"]);
                    quotation.FunctionName = Request.Form["FunctionName"];
                    quotation.CandidCinematographers = string.IsNullOrEmpty(Request.Form["CandidCinemato"])? 0 : Convert.ToInt32(Request.Form["CandidCinemato"]);
                    quotation.RegularCinematographers = string.IsNullOrEmpty(Request.Form["RegularCinemato"]) ? 0 : Convert.ToInt32(Request.Form["RegularCinemato"]);
                    quotation.CandidPhotographer = string.IsNullOrEmpty(Request.Form["CandidPhoto"]) ? 0 : Convert.ToInt32(Request.Form["CandidPhoto"]);
                    quotation.RegularPhotographer = string.IsNullOrEmpty(Request.Form["RegularPhoto"]) ? 0 : Convert.ToInt32(Request.Form["RegularPhoto"]);
                    quotation.DSLR = string.IsNullOrEmpty(Request.Form["DSLR"]) ? 0 : Convert.ToInt32(Request.Form["DSLR"]);
                    quotation.NumberOfDrones = string.IsNullOrEmpty(Request.Form["Drones"]) ? 0 : Convert.ToInt32(Request.Form["Drones"]);
                    quotation.NumberOfLedScreens = string.IsNullOrEmpty(Request.Form["LEDScreen"]) ? 0 : Convert.ToInt32(Request.Form["LEDScreen"]);
                    quotation.Others = string.IsNullOrEmpty(Request.Form["Other"]) ? string.Empty : Request.Form["Other"];
                    quotation.TotalAmount = Convert.ToDecimal(Request.Form["TotalAmount"]);

                    if (AAVEC.IsDisscount == true)
                    {
                        AAVEC.OriginalAmount  += Convert.ToDecimal(quotation.TotalAmount);

                        decimal DiscountedAmount = (Convert.ToDecimal(AAVEC.OriginalAmount) * Convert.ToDecimal(AAVEC.DiscountPercentage)) / 100;
                        AAVEC.DiscountedAmount = DiscountedAmount;
                        AAVEC.FinalAmount = AAVEC.OriginalAmount - DiscountedAmount;
                    }
                    else
                    {
                        AAVEC.OriginalAmount += Convert.ToDecimal(quotation.TotalAmount);
                        AAVEC.FinalAmount = AAVEC.OriginalAmount;
                    }
                    quotation.CreatedDate = DateTime.Now;
                    db.Entry(AAVEC).State = EntityState.Modified;
                    db.SaveChanges();
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
        
        [HttpPost]
        public ActionResult AddEmployee()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    int QID = Convert.ToInt32(Request.Form["QID"]);
                    int PGID = Convert.ToInt32(Request.Form["PGID"]);
                    int TypeID = Convert.ToInt32(Request.Form["TypeID"]);
                    if(IsEmployeeRecordExist(QID,PGID,TypeID))
                        return Json(new { IsSelected = true, message = "Photographer is already selected for this type" }, JsonRequestBehavior.AllowGet);
                    
                    tblOrder order = new tblOrder();
                    order.QuotationID = QID;
                    order.PhotographerID= PGID;
                    order.PhotographerTypeID = TypeID;
                    order.TotalPay = Convert.ToDecimal(Request.Form["Salary"]);
                    order.CreatedDate = DateTime.Now;
                    db.tblOrders.Add(order);
                    db.SaveChanges();
                    tblPayment payment = new tblPayment();
                    if (db.tblPayments.Where(P => P.PhotographerID == PGID).Count() > 0)
                    {
                        payment = db.tblPayments.SingleOrDefault(P => P.PhotographerID == PGID);
                        payment.RemainingPay += Convert.ToDecimal(Request.Form["Salary"]);
                        db.Entry(payment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        payment.PhotographerID = PGID;
                        payment.TotalPay = 0;
                        payment.RemainingPay = Convert.ToDecimal(Request.Form["Salary"]);
                        db.tblPayments.Add(payment);
                        db.SaveChanges();
                    }
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

        public bool IsEmployeeRecordExist(int QID,int PGID,int TypeID)
        {
            int RecordCount = db.tblOrders.Where(O => O.QuotationID == QID)
                                          .Where(O => O.PhotographerID == PGID)
                                          .Where(O => O.PhotographerTypeID == TypeID).Count();
            if (RecordCount > 0)
                return true;
            else
                return false;
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
                    AAVEC.AlbumPage = Convert.ToInt32(Request.Form["AlbumPage"]);
                    AAVEC.AlbumSize = Request.Form["AlbumSize"];
                    AAVEC.AlbumPrice = Convert.ToDecimal(Request.Form["AlbumPrice"]);
                    AAVEC.AlbumPadSize = Request.Form["AlbumPadSize"];
                    AAVEC.AlbumPadPrice = Convert.ToDecimal(Request.Form["AlbumPadPrice"]);
                    AAVEC.PhotoLEDFramePrice = Convert.ToDecimal(Request.Form["PhotoLEDFramePrice"]);
                    AAVEC.SpecialAlbum = Convert.ToDecimal(Request.Form["SpecialAlbum"]);
                    AAVEC.HDVideoDubbPendrivePrice = Convert.ToDecimal(Request.Form["HDVideoDubbPendrivePrice"]);
                    AAVEC.Other = Request.Form["Other"];
                    AAVEC.IsDisscount = Request.Form["IsDisscount"] == "true" ? true : false;
                    if(AAVEC.IsDisscount != true)
                    {
                        AAVEC.DiscountPercentage = 0;
                        AAVEC.DiscountedAmount = 0;
                        AAVEC.FinalAmount = AAVEC.OriginalAmount;
                    }
                    else
                    {
                        AAVEC.DiscountPercentage = string.IsNullOrEmpty(Request.Form["DiscountPercentage"]) ? 0 : Convert.ToInt32(Request.Form["DiscountPercentage"]);
                        AAVEC.OriginalAmount = string.IsNullOrEmpty(Request.Form["OriginalAmount"]) ? 0 : Convert.ToDecimal(Request.Form["OriginalAmount"]);
                        AAVEC.DiscountedAmount = Convert.ToDecimal(Request.Form["DiscountedAmount"]);
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
        public ActionResult DeleteQuotation(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblQuotation quotation = db.tblQuotations.Find(id);
                long AAVECID = Convert.ToInt64(quotation.AlbumAndVideoEditingChargesID);
                tblAlbumAndVideoEditingCharge AAVEC = db.tblAlbumAndVideoEditingCharges.SingleOrDefault(A => A.AlbumAndVideoEditingChargesID == AAVECID);
                
                if(AAVEC.IsDisscount == true)
                {
                    AAVEC.OriginalAmount -= Convert.ToDecimal(quotation.TotalAmount);
                    decimal DiscountedAmount = (Convert.ToDecimal(AAVEC.OriginalAmount) * Convert.ToDecimal(AAVEC.DiscountPercentage)) / 100;
                    AAVEC.DiscountedAmount = DiscountedAmount;
                    AAVEC.FinalAmount = AAVEC.OriginalAmount - DiscountedAmount;
                }
                else
                {
                    AAVEC.OriginalAmount -= Convert.ToDecimal(quotation.TotalAmount);
                    AAVEC.FinalAmount -= Convert.ToDecimal(quotation.TotalAmount);
                }
                db.Entry(AAVEC).State = EntityState.Modified;
                db.SaveChanges();

                db.tblQuotations.Remove(quotation);
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

        public bool IsQuotationIsExist(long CustomerID)
        {
            int RecordCount = db.tblAlbumAndVideoEditingCharges.Where(A => A.CustomerID == CustomerID).Count();
            if ( RecordCount > 0)
                 return true;
            else
                return false;
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
