using Microsoft.Ajax.Utilities;
using PhotoStudio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace PhotoStudio.Controllers
{
    public class UserPortfolioController : Controller
    {
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();

        public static int _PortfolioID = 0;

        // GET: UserPortfolio
        public ActionResult Registration()
        {
            int id = Convert.ToInt32(EncryptionDecryption.DecryptString(Request.QueryString["id"]));
            if(id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            else
            {
                _PortfolioID = id;
                return View("Registration");
            }
        }

        public ActionResult Index()
        {
            if (CheckSessionAndCookies())
                return View("Index");
            else
                return View("Registration");
        }


        public ActionResult CategoryWiseImages(int id=0)
        {
            if (CheckSessionAndCookies())
            {
                int CategoryID = id; //Convert.ToInt32(Request.Form["PGC"]);
                //int PortfolioID = Convert.ToInt32(Session["PortfolioID"]);
                var Images = db.tblPortfolioGalleries.Where(PG => PG.tblPortfolio.PortfolioID == _PortfolioID).Where(PG => PG.tblPortfolioGalleryCategory.PortfolioGalleryCategoryID == CategoryID);
                return View(Images.ToList());
            }
            else
                return View("Registration");
        }

        public bool CheckSessionAndCookies()
        {
            if (Session["PUEmail"] == null || Session["PUPhoneNumber"] == null || Session["PUName"] == null /*|| Session["PortfolioID"] == null*/)
            {
                if (Request.Cookies["PUEmail"] == null || Request.Cookies["PUPhoneNumber"] == null || Request.Cookies["PUName"] == null)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }
        [HttpPost]
        public ActionResult CheckSessionAndCookiesJson()
        {
            if (Session["PUEmail"] == null || Session["PUPhoneNumber"] == null || Session["PUName"] == null /*|| Session["PortfolioID"] == null*/)
            {
                if (Request.Cookies["PUEmail"] == null || Request.Cookies["PUPhoneNumber"] == null || Request.Cookies["PUName"] == null)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { success = true }, JsonRequestBehavior.AllowGet); ;
        }


        [HttpPost]
        public ActionResult getCategoryList()
        {

            //int PortfolioID = Convert.ToInt32(Session["PortfolioID"]);
            return Json(db.tblPortfolioGalleries.Where(C => C.tblPortfolio.PortfolioID == _PortfolioID).DistinctBy(C => C.tblPortfolioGalleryCategory.PortfolioGalleryCategoryID).Select(Q => new
            {
                CategoryID = Q.tblPortfolioGalleryCategory.PortfolioGalleryCategoryID,
                CategoryName = Q.tblPortfolioGalleryCategory.PortfolioGalleryCategoryName

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertVisitor()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string PhoneNumber = Request.Form["PhoneNo"];

                    if(IsUserExist(PhoneNumber))
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);

                    tblPortfolioVisitor visitors = new tblPortfolioVisitor();
                    visitors.PortfolioVisitorName = Request.Form["CustomerName"];
                    visitors.PortfolioVisitorEmail = Request.Form["CustomerEmail"];
                    visitors.PortfolioVisitorPhoneNumber = Request.Form["PhoneNo"];
                    visitors.PortfolioID = Convert.ToInt32(Session["PortfolioID"]);
                    visitors.VisitDate = DateTime.Now;
                    db.tblPortfolioVisitors.Add(visitors);
                    db.SaveChanges();
                    Session["PUEmail"] = visitors.PortfolioVisitorEmail;
                    Session["PUPhoneNumber"] = visitors.PortfolioVisitorPhoneNumber;
                    Session["PUName"] = visitors.PortfolioVisitorName;
                    Session["PortfolioID"] = visitors.PortfolioID;

                    Response.Cookies["PUEmail"].Value = visitors.PortfolioVisitorEmail;
                    Response.Cookies["PUPhoneNumber"].Value = visitors.PortfolioVisitorPhoneNumber;
                    Response.Cookies["PUName"].Value = visitors.PortfolioVisitorName;
                    Response.Cookies["PortfolioID"].Value = visitors.PortfolioID.ToString();

                    return Json(new { success = true}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { success = true, message = "Record inserted" }, JsonRequestBehavior.AllowGet);
        }

        public bool IsUserExist(string PhoneNumber)
        {
            var visitor = db.tblPortfolioVisitors.SingleOrDefault(P => P.PortfolioVisitorPhoneNumber == PhoneNumber);
            if (db.tblPortfolioVisitors.Where(P => P.PortfolioVisitorPhoneNumber == PhoneNumber).Count() > 0)
            {
                Session["PUEmail"] = visitor.PortfolioVisitorEmail;
                Session["PUPhoneNumber"] = visitor.PortfolioVisitorPhoneNumber;
                Session["PUName"] = visitor.PortfolioVisitorName;
                Session["PortfolioID"] = visitor.PortfolioID;
                return true;
            }
            else
                return false;
        }
    }
}