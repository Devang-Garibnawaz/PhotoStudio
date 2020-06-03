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
    public class CustomersController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.tblCustomers.ToList());
        }

        public ActionResult getAllActiveCustomers()
        {
            return Json(db.tblCustomers.Where(c => c.IsActive == true).Select(c => new
            {
                id = c.CustomerID,
                name = c.CustomerName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertCustomers()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EncryptionDecryption ED = new EncryptionDecryption();
                    tblCustomer newCust = new tblCustomer();
                    newCust.CustomerName = Request.Form["CustomerName"];
                    newCust.Email = Request.Form["Email"];
                    newCust.PhoneNumber = Request.Form["PhoneNumber"];
                    string password = ED.EncryptString(Request.Form["Password"]);

                    newCust.Password = password;
                    newCust.IsActive = Request.Form["IsActive"] == "true" ? true : false;
                    newCust.CreatedDate = DateTime.Now;

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
                            file.SaveAs(Server.MapPath("~/CustomerProfile/") + fileName);
                            if (!ImageProcessing.InsertImages(Server.MapPath("~/CustomerProfile/") + fileName))
                            {
                                return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion
                        }
                        newCust.ProfilePic = fileName;
                    }
                    db.tblCustomers.Add(newCust);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Record inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!"+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        [HttpPost]
        public ActionResult EditCustomer()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int CustomerID = Convert.ToInt32(Request.Form["CustomerID"]);
                    tblCustomer newCust = db.tblCustomers.SingleOrDefault(c => c.CustomerID == CustomerID);
                    newCust.CustomerName = Request.Form["CustomerName"];
                    newCust.Email = Request.Form["Email"];
                    newCust.PhoneNumber = Request.Form["PhoneNumber"];
                    newCust.IsActive = Request.Form["IsActive"] == "true" ? true : false;

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
                        //WebImage img = new WebImage(file.InputStream);

                        #region Save And compress file
                        //To save file, use SaveAs method
                        file.SaveAs(Server.MapPath("~/CustomerProfile/") + fileName);


                        if (!ImageProcessing.InsertImages(Server.MapPath("~/CustomerProfile/") + fileName))
                        {
                            return Json(new { success = false, message = "Error occur while uploading image." }, JsonRequestBehavior.AllowGet);
                        }
                        string path = Server.MapPath("~/CustomerProfile/" + newCust.ProfilePic);
                        if (newCust.ProfilePic != "" && newCust.ProfilePic != null && newCust.ProfilePic.Length > 0)
                        {
                            FileInfo delfile = new FileInfo(path);
                            delfile.Delete();
                        }
                        #endregion
                        newCust.ProfilePic = fileName;
                    }
                    newCust.UpdatedDate = DateTime.Now;
                    db.Entry(newCust).State = EntityState.Modified;
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
        public ActionResult DeleteCustomer(int id)
        {
            try
            {
                tblCustomer tblCustomer = db.tblCustomers.Find(id);
                string path = Server.MapPath("~/CustomerProfile/" + tblCustomer.ProfilePic);
                FileInfo delfile = new FileInfo(path);
                delfile.Delete();
                db.tblCustomers.Remove(tblCustomer);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!"+ ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ActivateDeactivateCustomer()
        {
            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                tblCustomer newCust = db.tblCustomers.SingleOrDefault(c => c.CustomerID == id);
                newCust.UpdatedDate = DateTime.Now;
                if (Flag >= 1)
                    newCust.IsActive = true;
                else
                    newCust.IsActive = false;
                db.Entry(newCust).State = EntityState.Modified;
                db.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Customer is activated" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { DeAct = true, message = "Customer is de-activated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Error = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IsPhoneNumberExist()
        {
            try
            {
                string PhoneNumber = Request.Form["PhoneNumber"];
                if (string.IsNullOrEmpty(PhoneNumber))
                    return Json(new { success = false, message = "Phone number is not supplied!" }, JsonRequestBehavior.AllowGet);

                if (db.tblCustomers.Any(C => C.PhoneNumber == PhoneNumber))
                    return Json(new { success = true, message = "Phone number already registered!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, message = "Record Not Existed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
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
