using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotoStudio.Models;

namespace PhotoStudio.Controllers
{
    public class BannerController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View(db.tblbanners.ToList());
        }

        [HttpPost]
        public ActionResult InsertBanner()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            try
            {
                if (ModelState.IsValid)
                {
                    tblbanner newBanner = new tblbanner();
                    newBanner.BannerHeading = Request.Form["BannerHeading"];
                    newBanner.BannerDescription = Request.Form["BannerDescription"];

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


                            if (mimeType.ToLower() != "image/jpeg" && mimeType.ToLower() != "image/jpg")
                            {
                                return Json(new { success = false, message = "Banner Image format must be JPEG or JPG" }, JsonRequestBehavior.AllowGet);
                            }

                            #region Save And compress file
                            //To save file, use SaveAs method
                            file.SaveAs(Server.MapPath("~/BannerImages/") + fileName);
                            if (!ImageProcessing.InsertPortfolioImages(Server.MapPath("~/BannerImages/") + fileName))
                            {
                                return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                            }
                            newBanner.BannerImage = fileName;
                            #endregion
                        }
                        else
                        {
                            return Json(new { success = false, message = "Image is not selected." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    db.tblbanners.Add(newBanner);
                    db.SaveChanges();

                }
                return Json(new { success = true, message = "Banner inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult DeleteBanner(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            try
            {
                tblbanner tblBanner = db.tblbanners.Find(id);
                db.tblbanners.Remove(tblBanner);
                db.SaveChanges();
                string path = Server.MapPath("~/BannerImages/" + tblBanner.BannerImage);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
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
