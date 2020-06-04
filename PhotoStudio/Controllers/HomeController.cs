using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoStudio.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserID"] == null && Session["UserName"] == null)
                return RedirectToAction("Login", "Login");

            return View();
        }

      
    }
}