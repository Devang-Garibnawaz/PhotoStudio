﻿using Microsoft.Ajax.Utilities;
using PhotoStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            HttpCookie cookie = GetPortfolioCookie();
            int id = Convert.ToInt32(EncryptionDecryption.DecryptString(Request.QueryString["id"]));
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (db.tblPortfolios.SingleOrDefault(P => P.PortfolioID == id).IsActive == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                if (cookie != null)
                {
                    Session["PUEmail"] = cookie["PUEmail"];
                    Session["PUPhoneNumber"] = cookie["PUPhoneNumber"];
                    Session["PUName"] = cookie["PUName"];
                    ViewBag.BannerImage = getRandomBanner();
                    _PortfolioID = id;
                    return View("Index", db.tblbanners.ToList());
                }

                ViewBag.BannerImage = getRandomBanner();
                _PortfolioID = id;
                return View("Registration");
            }
        }

        public ActionResult Index()
        {
            if (CheckSessionAndCookies())
                return View("Index",db.tblbanners.ToList());
            else
                return View("Registration");
        }


        public ActionResult CategoryWiseImages(string strid)
        {
            int id = Convert.ToInt32(EncryptionDecryption.DecryptString(strid));
            if(id <= 0)
                return View("Registration");

            if (CheckSessionAndCookies())
            {
                ViewBag.BannerImage = getRandomBanner();
                int CategoryID = id;
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
                if (GetPortfolioCookie() != null)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        [HttpPost]
        public ActionResult CheckSessionAndCookiesJson()
        {
            if (Session["PUEmail"] == null || Session["PUPhoneNumber"] == null || Session["PUName"] == null /*|| Session["PortfolioID"] == null*/)
            {
                if (GetPortfolioCookie() != null)
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { success = true }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public ActionResult getCategoryList()
        {
            return Json(db.tblPortfolioGalleries.Where(C => C.tblPortfolio.PortfolioID == _PortfolioID).DistinctBy(C => C.tblPortfolioGalleryCategory.PortfolioGalleryCategoryID).Select(Q => new
            {
                CategoryID = EncryptionDecryption.EncryptString(Q.tblPortfolioGalleryCategory.PortfolioGalleryCategoryID.ToString()),
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
                    visitors.CustomerName = db.tblPortfolios.SingleOrDefault(P => P.PortfolioID == _PortfolioID).tblCustomer.CustomerName;
                    visitors.VisitDate = DateTime.Now;
                    db.tblPortfolioVisitors.Add(visitors);
                    db.SaveChanges();
                    Session["PUEmail"] = visitors.PortfolioVisitorEmail;
                    Session["PUPhoneNumber"] = visitors.PortfolioVisitorPhoneNumber;
                    Session["PUName"] = visitors.PortfolioVisitorName;

                    SetPortfolioCookie(visitors.PortfolioVisitorEmail,
                                          visitors.PortfolioVisitorPhoneNumber,
                                          visitors.PortfolioVisitorName);

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
                return true;
            }
            else
                return false;
        }

        private void SetPortfolioCookie(string Email,string PhoneNumber,string Name)
        {
            HttpCookie PortfolioCookie = new HttpCookie("PortfolioCookie");
            PortfolioCookie["PUEmail"] = Email;
            PortfolioCookie["PUEmail"] = PhoneNumber;
            PortfolioCookie["PUEmail"] = Name;
            Response.Cookies.Add(PortfolioCookie);
        }
        
        private HttpCookie GetPortfolioCookie()
        {
            HttpCookie cookie = Request.Cookies["PortfolioCookie"];
            if (cookie != null)
                return cookie;
            else
                return null;
        }
        public string getRandomBanner()
        {
            string file = null;
            var extensions = new string[] { ".jpeg", ".jpg" };
            try
            {
                var di = new DirectoryInfo(Server.MapPath("~/BannerImages/"));
                var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                Random R = new Random();
                file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).Name;
            }
            // probably should only catch specific exceptions
            // throwable by the above methods.
            catch (Exception ex) { }
            return file;
        }
    }
}