using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using PhotoStudio.Models;

namespace PhotoStudio.Controllers
{
    public class PortfolioController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Portfolio
        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            var tblPortfolios = db.tblPortfolios.Include(t => t.tblCustomer);
            return View(tblPortfolios.ToList());
        }

        [HttpPost]
        public ActionResult InsertPortfolio()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    tblPortfolio portfolio = new tblPortfolio();
                    int CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    
                    if (IsPortfolioExist(CustomerID))
                        return Json(new { success = false, message = "Portfolio of selected customer is already exist!" }, JsonRequestBehavior.AllowGet);
                    
                    portfolio.CustomerID = CustomerID;
                    portfolio.PortfolioHeading = Request.Form["Heading"];
                    portfolio.PortfolioDescription = Request.Form["Description"];
                    portfolio.IsActive = Request.Form["IsActive"] == "true" ? true : false;
                    portfolio.CreatedDate = DateTime.Now;

                    if (ModelState.IsValid)
                    {
                        int fileSize = 0;
                        string fileName = string.Empty;
                        string mimeType = string.Empty;
                        System.IO.Stream fileContent;

                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase file = Request.Files[0];

                            fileSize = file.ContentLength;
                            fileName = file.FileName;
                            mimeType = file.ContentType;
                            fileContent = file.InputStream;


                            if (mimeType.ToLower() != "image/jpeg" && mimeType.ToLower() != "image/jpg" && mimeType.ToLower() != "image/png")
                            {
                                return Json(new { Formatwarning = true, message = "Profile pic format must be JPEG or JPG or PNG." }, JsonRequestBehavior.AllowGet);
                            }

                            #region Save And compress file
                            //To save file, use SaveAs method
                            file.SaveAs(Server.MapPath("~/PortfolioCoverImage/") + fileName);
                            if (!ImageProcessing.InsertImages(Server.MapPath("~/PortfolioCoverImage/") + fileName))
                            {
                                return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion
                        }
                        portfolio.CoverPhoto = fileName;
                    }
                    db.tblPortfolios.Add(portfolio);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Record inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeletePortfolio(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblPortfolio portfolio = db.tblPortfolios.Find(id);
                string path = Server.MapPath("~/PortfolioCoverImage/" + portfolio.CoverPhoto);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.tblPortfolios.Remove(portfolio);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool IsPortfolioExist(int CustomerID)
        {
            if (db.tblPortfolios.Where(P => P.CustomerID == CustomerID).Count() > 0)
                return true;
            else
                return false;
        }

        public ActionResult ActivateDeactivateFolio()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                tblPortfolio portfolio = db.tblPortfolios.SingleOrDefault(c => c.PortfolioID == id);
                
                if (Flag >= 1)
                    portfolio.IsActive = true;
                else
                    portfolio.IsActive = false;
                db.Entry(portfolio).State = EntityState.Modified;
                db.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Portfolio is activated" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { DeAct = true, message = "Portfolio is de-activated" }, JsonRequestBehavior.AllowGet);
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
