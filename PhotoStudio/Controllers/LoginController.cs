using PhotoStudio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstaAlbum.Controllers
{
    public class LoginController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }
        public ActionResult CheckUserIsExistsOrNot()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EncryptionDecryption ED = new EncryptionDecryption();
                    string strPhno = Request.Form["PhoneNo"];
                    string strPassword =  ED.EncryptString(Request.Form["Password"]);

                    var data1 = db.tblCustomers.Where(c => c.PhoneNumber == strPhno.Trim())
                               .Where(c => c.Password == strPassword.Trim()).Where(c => c.IsActive == true).ToList();
                    
                    var data2 = db.tblSystemUsers.Where(a => a.PhoneNumber == strPhno.Trim())
                               .Where(c => c.Password == strPassword.Trim()).ToList();

                    if (data1.Count > 0)
                    {
                        foreach (tblCustomer cust in data1)
                        {
                            Session["CustomerID"] = cust.CustomerID;
                            Session["CustomerName"] = cust.CustomerName;
                            Session["CustomerPhoneNumber"] = cust.PhoneNumber;
                        }
                        return Json(new { UserExist = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else if(data2.Count > 0)
                    {
                        foreach (tblSystemUser SU in data2)
                        {
                            Session["UserID"] = SU.UserID;
                            Session["UserName"] = SU.UserName;
                        }
                        return Json(new { AdminExist = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { UserExist = false, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { UserExist = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { UserExist = true, message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsClientPhnoExistOrNot(string Phno)
        {
            if (db.tblCustomers.Any(c => c.PhoneNumber == Phno))
                return Json(new { success = true, message = "Record Existed" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false, message = "Record Not Existed" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LogOut()
        {
            try
            {
                Session.RemoveAll();
                Session.Abandon();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}