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
    public class CategoryController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Category
        public ActionResult Index()
        {
            return View(db.tblCategories.ToList());
        }



        public ActionResult getAllCategory()
        {
            return Json(db.tblCategories.Select(c => new
            {
                id = c.CategoryID,
                name = c.CategoryName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertCategory()
        {

            try
            {
                tblCategory newCat = new tblCategory();
                newCat.CategoryName = Request.Form["CategoryName"];

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
                        file.SaveAs(Server.MapPath("~/CategoryImages/") + fileName);
                        if (!ImageProcessing.InsertImages(Server.MapPath("~/CategoryImages/") + fileName))
                        {
                            return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                    newCat.CategoryImage = fileName;

                    newCat.CreatedDate = DateTime.Now;
                    db.tblCategories.Add(newCat);
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
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                tblCategory tblCategory = db.tblCategories.Find(id);
                string path = Server.MapPath("~/CategoryImages/" + tblCategory.CategoryImage);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.tblCategories.Remove(tblCategory);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occur!"+ex.Message }, JsonRequestBehavior.AllowGet);
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
