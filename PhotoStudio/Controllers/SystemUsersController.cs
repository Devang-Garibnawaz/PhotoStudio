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
            return View(db.tblSystemUsers.ToList());
        }

        // GET: SystemUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSystemUser tblSystemUser = db.tblSystemUsers.Find(id);
            if (tblSystemUser == null)
            {
                return HttpNotFound();
            }
            return View(tblSystemUser);
        }

        // GET: SystemUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult InsertSystemUser()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EncryptionDecryption ED = new EncryptionDecryption(); 
                    tblSystemUser SU = new tblSystemUser();
                    SU.UserName = Request.Form["UserName"];

                    string password = ED.EncryptString(Request.Form["Password"]);

                    SU.Password = password;
                    SU.Email = Request.Form["Email"];
                    SU.PhoneNumber = Request.Form["PhoneNumber"];
                    SU.IsActive = true;
                    SU.CreatedDate = DateTime.Now;

                    db.tblSystemUsers.Add(SU);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Operation done successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Invalid Model State");
                }
            }
            catch(Exception ex)
            {
                return Json(new { success = false,message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: SystemUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSystemUser tblSystemUser = db.tblSystemUsers.Find(id);
            if (tblSystemUser == null)
            {
                return HttpNotFound();
            }
            return View(tblSystemUser);
        }

        // POST: SystemUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,Email,Password,PhoneNumber,IsActive,CreatedDate,UpdatedDate")] tblSystemUser tblSystemUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblSystemUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblSystemUser);
        }

        // GET: SystemUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSystemUser tblSystemUser = db.tblSystemUsers.Find(id);
            if (tblSystemUser == null)
            {
                return HttpNotFound();
            }
            return View(tblSystemUser);
        }

        // POST: SystemUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSystemUser tblSystemUser = db.tblSystemUsers.Find(id);
            db.tblSystemUsers.Remove(tblSystemUser);
            db.SaveChanges();
            return RedirectToAction("Index");
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
