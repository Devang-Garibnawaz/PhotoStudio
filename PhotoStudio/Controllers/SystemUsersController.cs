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
    public class SystemUsersController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: SystemUsers
        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View(db.tblSystemUsers.ToList());
        }

        [HttpPost]
        public ActionResult InsertSystemUser()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                if (ModelState.IsValid)
                {
                    tblSystemUser SU = new tblSystemUser();
                    SU.UserName = Request.Form["UserName"];

                    string password = EncryptionDecryption.EncryptString(Request.Form["Password"]);

                    SU.Password = password;
                    SU.Email = Request.Form["Email"];
                    SU.PhoneNumber = Request.Form["PhoneNumber"];
                    SU.IsActive = true;
                    SU.CreatedDate = DateTime.Now;

                    db.tblSystemUsers.Add(SU);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Record inserted successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Invalid Model State");
                }
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblSystemUser tblSystemUser = db.tblSystemUsers.Find(id);
                db.tblSystemUsers.Remove(tblSystemUser);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Error-->"+ex.Message }, JsonRequestBehavior.AllowGet);
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

        public ActionResult IsPhoneNumberExist()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                string PhoneNumber = Request.Form["PhoneNumber"];
                if(string.IsNullOrEmpty(PhoneNumber))
                    return Json(new { success = false, message = "Phone number is not supplied!" }, JsonRequestBehavior.AllowGet);
                
                if (db.tblSystemUsers.Any(SU => SU.PhoneNumber == PhoneNumber))
                    return Json(new { success = true, message = "Phone number already registered!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, message = "Record Not Existed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
