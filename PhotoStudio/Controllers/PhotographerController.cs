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
    public class PhotographerController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Photographer
        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View(db.tblPhotographers.ToList());
        }

        // GET: Photographer/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View();
        }

        [HttpPost]
        public ActionResult InsertPhotographer()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    tblPhotographer PHG = new tblPhotographer();
                    PHG.PhotographerName = Request.Form["PhotographerName"];
                    PHG.Email = Request.Form["Email"];
                    PHG.PhoneNumber = Request.Form["PhoneNumber"];
                    PHG.Address = Request.Form["Address"]; ;
                    PHG.Salary = Convert.ToDecimal(Request.Form["Salary"]);
                    PHG.IsActive = Request.Form["IsActive"] == "true" ? true : false;
                    PHG.IsFreeLancer = Request.Form["IsFreelancer"] == "true" ? true : false;
                    PHG.CreatedDate = DateTime.Now;

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
                            file.SaveAs(Server.MapPath("~/PhotographerProfilePic/") + fileName);
                            if (!ImageProcessing.InsertImages(Server.MapPath("~/PhotographerProfilePic/") + fileName))
                            {
                                return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion
                        }
                        PHG.ProfilePic = fileName;
                    }
                    db.tblPhotographers.Add(PHG);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Record inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Photographer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPhotographer tblPhotographer = db.tblPhotographers.Find(id);
            if (tblPhotographer == null)
            {
                return HttpNotFound();
            }
            return View(tblPhotographer);
        }

        [HttpPost]
        public ActionResult EditPhotographer()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    int PhotographerID = Convert.ToInt32(Request.Form["PhotographerID"]);
                    tblPhotographer PGH = db.tblPhotographers.SingleOrDefault(c => c.PhotographerID == PhotographerID);
                    PGH.PhotographerName = Request.Form["PhotographerName"];
                    PGH.Email = Request.Form["Email"];
                    PGH.PhoneNumber = Request.Form["PhoneNumber"];
                    PGH.Address = Request.Form["Address"]; ;
                    PGH.Salary = Convert.ToDecimal(Request.Form["Salary"]);
                    PGH.IsActive = Request.Form["IsActive"] == "true" ? true : false;
                    PGH.IsFreeLancer = Request.Form["IsFreeLancer"] == "true" ? true : false;

                    if (Request.Files.Count > 0)
                    {
                        int fileSize = 0;
                        string fileName = string.Empty;
                        string mimeType = string.Empty;
                        System.IO.Stream fileContent;

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
                        file.SaveAs(Server.MapPath("~/PhotographerProfilePic/") + fileName);


                        if (!ImageProcessing.InsertImages(Server.MapPath("~/PhotographerProfilePic/") + fileName))
                        {
                            return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                        }
                        string path = Server.MapPath("~/PhotographerProfilePic/" + PGH.ProfilePic);
                        if (PGH.ProfilePic != "" && PGH.ProfilePic != null && PGH.ProfilePic.Length > 0)
                        {
                            FileInfo delfile = new FileInfo(path);
                            delfile.Delete();
                        }
                        #endregion
                        PGH.ProfilePic = fileName;
                    }
                    PGH.UpdatedDate = DateTime.Now;
                    db.Entry(PGH).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Record updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeletePhotographer(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblPhotographer photographer = db.tblPhotographers.Find(id);
                string path = Server.MapPath("~/PhotographerProfilePic/" + photographer.ProfilePic);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.tblPhotographers.Remove(photographer);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ActivateDeactivatePhotographer()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                tblPhotographer PHG = db.tblPhotographers.SingleOrDefault(P => P.PhotographerID == id);
                PHG.UpdatedDate = DateTime.Now;
                if (Flag >= 1)
                    PHG.IsActive = true;
                else
                    PHG.IsActive = false;
                db.Entry(PHG).State = EntityState.Modified;
                db.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Photographer is activated" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { DeAct = true, message = "Photographer is de-activated" }, JsonRequestBehavior.AllowGet);
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
