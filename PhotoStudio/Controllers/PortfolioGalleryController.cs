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
    public class PortfolioGalleryController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: PortfolioGallery
        public ActionResult Index(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            if (id == null)
            {
                ViewBag.PortfolioID = id;
                var tblPortfolioGalleries = db.tblPortfolioGalleries.Include(t => t.tblPortfolio).Include(t => t.tblPortfolioGalleryCategory).Where(PF => PF.tblPortfolio.IsActive == true);
                return View(tblPortfolioGalleries.ToList());
            }
            else
            {
                var tblPortfolioGalleries = db.tblPortfolioGalleries.Include(t => t.tblPortfolio).Include(t => t.tblPortfolioGalleryCategory).Where(PF => PF.tblPortfolio.IsActive == true).Where(PF => PF.PortfolioID == id);
                return View(tblPortfolioGalleries.ToList());
            }
        }

        public ActionResult PortfolioGalleryCategory()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View(db.tblPortfolioGalleryCategories.ToList());
        }

        public ActionResult getAllCategory()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return Json(db.tblPortfolioGalleryCategories.Select(c => new
            {
                id = c.PortfolioGalleryCategoryID,
                name = c.PortfolioGalleryCategoryName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertPortfolioCategory()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblPortfolioGalleryCategory PGC = new tblPortfolioGalleryCategory();
                PGC.PortfolioGalleryCategoryName = Request.Form["CategoryName"];

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
                        file.SaveAs(Server.MapPath("~/PortfolioCategory/") + fileName);
                        if (!ImageProcessing.InsertImages(Server.MapPath("~/PortfolioCategory/") + fileName))
                        {
                            return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                    PGC.PortfolioGalleryCategoryImage = fileName;

                    PGC.CreatedDate = DateTime.Now;
                    db.tblPortfolioGalleryCategories.Add(PGC);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Record Not inserted" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { success = true, message = "Record inserted" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertImage()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    int PortfolioID = Convert.ToInt32(Request.Form["PortfolioID"]);
                    int CategoryID = Convert.ToInt32(Request.Form["CategoryID"]);
                    
                    if(IsGalleryCapacityOverflow(CategoryID,PortfolioID))
                        return Json(new { success = false, message = "Selected Category is already having 25 images!" }, JsonRequestBehavior.AllowGet);

                    if (Request.Files.Count > 0)
                    {
                        int fileSize = 0;
                        string fileName = string.Empty;
                        string mimeType = string.Empty;
                        System.IO.Stream fileContent;

                        if(Request.Files.Count > 25)
                            return Json(new { success = false, message = "You can not insert more than 25 Images in single category" }, JsonRequestBehavior.AllowGet);

                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            tblPortfolioGallery portfolioGallery = new tblPortfolioGallery();
                            bool ApplyWatermark = Request.Form["chkApplyWaterMark"] == "true" ? true : false;

                            portfolioGallery.PortfolioID = PortfolioID;
                            portfolioGallery.PortfolioGalleryCategoryID = CategoryID;

                            HttpPostedFileBase file = Request.Files[i];

                            fileSize = file.ContentLength;
                            fileName = file.FileName;
                            mimeType = file.ContentType;
                            fileContent = file.InputStream;


                            if (mimeType.ToLower() != "image/jpeg" && mimeType.ToLower() != "image/jpg" && mimeType.ToLower() != "image/png")
                            {
                                return Json(new { Formatwarning = true, message = "Profile pic format must be JPEG or JPG or PNG." }, JsonRequestBehavior.AllowGet);
                            }
                            //WebImage img = new WebImage(file.InputStream);

                            #region Save And compress file
                            //To save file, use SaveAs method
                            file.SaveAs(Server.MapPath("~/PortfolioGalleryImages/") + fileName);
                            #endregion
                            portfolioGallery.PortfolioGalleryImage = fileName;
                            portfolioGallery.CreatedDate = DateTime.Now;
                            db.tblPortfolioGalleries.Add(portfolioGallery);
                            db.SaveChanges();
                        }
                    }

                    return Json(new { success = true, message = "Record inserted successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Record not inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool IsGalleryCapacityOverflow(int CategoryID,int PortfolioID)
        {
            int intRecords = db.tblPortfolioGalleries.Where(PG => PG.PortfolioID == PortfolioID).Where(PG => PG.PortfolioGalleryCategoryID == CategoryID).Count();
            if (intRecords > 25)
                return true;
            else
                return false;
        }
        [HttpPost]
        public ActionResult DeletePortfolioCategory(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            try
            {
                tblPortfolioGalleryCategory PGC = db.tblPortfolioGalleryCategories.Find(id);
                string path = Server.MapPath("~/PortfolioCategory/" + PGC.PortfolioGalleryCategoryImage);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.tblPortfolioGalleryCategories.Remove(PGC);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occur!" + ex.Message }, JsonRequestBehavior.AllowGet);
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
