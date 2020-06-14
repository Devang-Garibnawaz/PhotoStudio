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
    public class OrdersController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Orders
        public ActionResult Index(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            if (id == null)
            {
                var tblOrders = db.tblOrders.Include(t => t.tblPhotographer).Include(t => t.tblPhotographerType).Include(t => t.tblQuotation);
                return View(tblOrders.ToList());
            }
            else
            {
                var tblOrders = db.tblOrders.Include(t => t.tblPhotographer).Include(t => t.tblPhotographerType).Include(t => t.tblQuotation);
                return View(tblOrders.Where(O => O.QuotationID == id).Where(O => O.tblQuotation.tblAlbumAndVideoEditingCharge.IsPass == true).ToList());
            }
        }

        public ActionResult PhotographerWiseOrder(int? id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tblOrders = db.tblOrders.Include(t => t.tblPhotographer).Include(t => t.tblPhotographerType).Include(t => t.tblQuotation);
            ViewBag.PhotographerName = db.tblPhotographers.SingleOrDefault(p => p.PhotographerID == id).PhotographerName; 
            return View(tblOrders.Where(O => O.PhotographerID == id).Where(O => O.tblQuotation.tblAlbumAndVideoEditingCharge.IsPass == true).ToList());
            
        }

        // POST: Orders/Delete/5
        [HttpPost]
        public ActionResult DeleteOrder(long id)
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            try
            {
                tblOrder order = db.tblOrders.Find(id);
                db.tblOrders.Remove(order);
                db.SaveChanges();
                return Json(new { success = true, message = "Record deleted successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error!" + ex.Message }, JsonRequestBehavior.AllowGet);
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
