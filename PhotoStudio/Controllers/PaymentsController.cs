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
    public class PaymentsController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        // GET: Payments
        public ActionResult Index()
        {
            var tblPayments = db.tblPayments.Include(t => t.tblPhotographer);
            return View(tblPayments.ToList());
        }

        public ActionResult PaymentHistory(int id=0)
        {
            if (id <= 0)
            {
                var tblPayments = db.tblPayments.Include(t => t.tblPhotographer);
                return View(tblPayments.ToList());
            }
            var tblPaymentHistory = db.tblPaymentHistories.Where(P => P.tblPhotographer.PhotographerID == id).Include(t => t.tblPhotographer);
            ViewBag.PhotographerName = db.tblPhotographers.SingleOrDefault(P => P.PhotographerID == id).PhotographerName;
            return View(tblPaymentHistory.ToList());
        }

        [HttpPost]
        public ActionResult MakePayment()
        {
            try
            {
                int PGID = Convert.ToInt32(Request.Form["PGID"]);
                decimal Amount = Convert.ToDecimal(Request.Form["Amount"]);
                tblPayment payment = db.tblPayments.SingleOrDefault(P => P.PhotographerID == PGID);

                if (payment.RemainingPay < Amount)
                    return Json(new { success = false, message = "Amount must be less than remaining pay!" }, JsonRequestBehavior.AllowGet);
                
                payment.TotalPay += Amount;
                payment.RemainingPay -= Amount;
                payment.PaymentDate = DateTime.Now;
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();

                tblPaymentHistory paymentHistory = new tblPaymentHistory();
                paymentHistory.PhotographerID = PGID;
                paymentHistory.TotalPay = Amount;
                paymentHistory.RemainingPay = payment.RemainingPay;
                paymentHistory.PaymetnDate = DateTime.Now;
                db.tblPaymentHistories.Add(paymentHistory);
                db.SaveChanges();

                return Json(new { success = true, message = "Payment Done!" }, JsonRequestBehavior.AllowGet);

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
