using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PhotoStudio.Models;

namespace PhotoStudio.Controllers
{
    public class GalleryController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Gallery
        public ActionResult GalleryList(int id=0)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            if (id <= 0)
                return View("GalleryCustomerList", db.tblCustomers.ToList());
            var tblGalleries = db.tblGalleries.Where(g => g.CustomerID == id);
            ViewBag.CustomerName = db.tblCustomers.SingleOrDefault(c => c.CustomerID == id).CustomerName;
            ViewBag.CustomerID = id.ToString();
            return View(tblGalleries.ToList());
        }

        public ActionResult GalleryCustomerList()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View(db.tblCustomers.Where(c => c.IsActive == true).ToList());
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
        public ActionResult getJsonFile()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            int CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
            int CategoryID = Convert.ToInt32(Request.Form["CategoryID"]);
            try
            {


                //7.
                var data = db.tblGalleries.Where(g => g.CustomerID == CustomerID)
                            .Where(g => g.CategoryID == CategoryID)
                            .Where(g => g.IsSelected == true)
                            .ToDictionary(g => g.GalleryID, g => g.Image).ToList();


                if(data.Count <= 0)
                {
                    return Json(new { success = false, message = "No file is selected! Please inform client to select images" }, JsonRequestBehavior.AllowGet);
                }
                string json = new JavaScriptSerializer().Serialize(data);
                if (!Directory.Exists(Server.MapPath("~/App_Data/")))
                    Directory.CreateDirectory(Server.MapPath("~/App_Data/"));

                string path = Server.MapPath("~/App_Data/");
                Random random = new Random();
                string Num = "";
                for (int i = 0; i < 4; i++)
                {
                    Num += random.Next(1, 9).ToString();
                }
                // Write that JSON to txt file,  
                System.IO.File.WriteAllText(path + Num + "_" + DateTime.Now.ToString("d-M-yyyy") + ".json", json);
                WebClient webClient = new WebClient();
                if (!Directory.Exists(@"C:\AllJsonFiles\"))
                {
                    Directory.CreateDirectory(@"C:\AllJsonFiles");
                }
                webClient.DownloadFile(path + Num + "_" + DateTime.Now.ToString("d-M-yyyy") + ".json", @"C:\AllJsonFiles\" + Num + "_" + DateTime.Now.ToString("d-M-yyyy") + ".json");
                FileInfo delfile = new FileInfo(path + Num + "_" + DateTime.Now.ToString("d-M-yyyy") + ".json");
                delfile.Delete();
                return Json(new { success = true, message = "File is created." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "File is not created." + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

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
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

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
