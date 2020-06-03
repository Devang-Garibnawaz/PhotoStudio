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
    public class GalleryController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Gallery
        public ActionResult GalleryList(int id=0)
        {
            if (id <= 0)
                return View("GalleryCustomerList", db.tblCustomers.ToList());
            var tblGalleries = db.tblGalleries.Where(g => g.CustomerID == id);
            ViewBag.CustomerName = db.tblCustomers.SingleOrDefault(c => c.CustomerID == id).CustomerName;
            ViewBag.CustomerID = id.ToString();
            return View(tblGalleries.ToList());
        }

        public ActionResult GalleryCustomerList()
        {
            return View(db.tblCustomers.Where(c => c.IsActive == true).ToList());
        }

        [HttpPost]
        public ActionResult InsertImage()
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (Request.Files.Count > 0)
                    {
                        int fileSize = 0;
                        string fileName = string.Empty;
                        string mimeType = string.Empty;
                        System.IO.Stream fileContent;

                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            tblGallery NewGallery = new tblGallery();
                            int CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                            int CategoryID = Convert.ToInt32(Request.Form["CategoryID"]);
                            bool ApplyWatermark = Request.Form["chkApplyWaterMark"] == "true" ? true : false;

                            NewGallery.CustomerID = CustomerID;
                            NewGallery.CategoryID = CategoryID;

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
                            file.SaveAs(Server.MapPath("~/GalleryImages/") + fileName);


                            if (ApplyWatermark)
                            {
                                if (!ImageProcessing.InsertImages(Server.MapPath("~/GalleryImages/") + fileName, true))
                                {
                                    return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                if (!ImageProcessing.InsertImages(Server.MapPath("~/GalleryImages/") + fileName))
                                {
                                    return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            #endregion

                            NewGallery.Image = fileName;
                            NewGallery.CreatedDate = DateTime.Now;
                            NewGallery.IsSelected = false;
                            db.tblGalleries.Add(NewGallery);
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
                return Json(new { success = false, message = "Error!"+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

       
        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            try
            {
                tblGallery tblGallery = db.tblGalleries.Find(id);
                db.tblGalleries.Remove(tblGallery);
                string path = Server.MapPath("~/GalleryImages/" + tblGallery.Image);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.SaveChanges();
                return Json(new { success = true, message = "Image is deleted." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Image is not deleted." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult DeleteSelectedImages(List<int> CheckedID)
        {
            try
            {
                // iterate through input list and pass to process method
                for (int i = 0; i < CheckedID.Count; i++)
                {
                    if (CheckedID[i] > 0)
                        DeleteImageByID(CheckedID[i]);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        

        public void DeleteImageByID(int id)
        {
            tblGallery tblGallery = db.tblGalleries.Find(id);
            db.tblGalleries.Remove(tblGallery);
            string path = Server.MapPath("~/GalleryImages/" + tblGallery.Image);
            FileInfo delfile = new FileInfo(path);
            delfile.Delete();
            db.SaveChanges();
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
