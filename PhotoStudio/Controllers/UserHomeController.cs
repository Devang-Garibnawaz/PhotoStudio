
using PhotoStudio.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstaAlbum.Controllers
{
    public class UserHomeController : Controller
    {
        // GET: UserHome
        private ManishPhotoStudioEntities db = new ManishPhotoStudioEntities();
        public ActionResult Index()
        {
            if (Session["CustomerID"] == null && Session["CustomerName"] == null && Session["CustomerPhoneNumber"] == null)
                return RedirectToAction("Login", "Login");
            
            return View();
        }
       
        public ActionResult Protfolio()
        {
                return View();
        }
        public ActionResult GalleryCategories()
        {
            if (Session["CustomerID"] == null && Session["CustomerName"] == null && Session["CustomerPhoneNumber"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View(db.tblCategories.ToList());
            }
        }
        public ActionResult CategoryWiseImage(int id)
        {
            if (Session["CustomerID"] == null && Session["CustomerName"] == null && Session["CustomerPhoneNumber"] == null)
                return RedirectToAction("Login", "Login");
            
            if (id <= 0 )
            {
                return RedirectToAction("GalleryCategories", "UserHome");
            }

            int CustomerID = Convert.ToInt32(Session["CustomerID"]);
            var data = db.tblGalleries.Where(g => g.CategoryID == id).Where(g => g.CustomerID == CustomerID);
            return View(data.ToList());
            
        }

        public void ChangeImageSelected(int id)
        {
            tblGallery objGallery = new tblGallery();
            objGallery = db.tblGalleries.SingleOrDefault(g => g.GalleryID == id);

            objGallery.IsSelected = true;
            db.Entry(objGallery).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void ChangeImageUnSelected(int id)
        {
            tblGallery objGallery = new tblGallery();
            objGallery = db.tblGalleries.SingleOrDefault(g => g.GalleryID == id);
            objGallery.IsSelected = false;
            db.Entry(objGallery).State = EntityState.Modified;
            db.SaveChanges();
        }
        public ActionResult SaveSelectedImages(List<int> CheckedID, List<int> UnCheckedID)
        {
            try
            {
                // iterate through input list and pass to process method
                for (int i = 0; i < CheckedID.Count; i++)
                {
                    if(CheckedID[i] > 0)
                        ChangeImageSelected(CheckedID[i]);  
                }

                for (int i = 0; i < UnCheckedID.Count; i++)
                {
                    if (UnCheckedID[i] > 0)
                        ChangeImageUnSelected(UnCheckedID[i]);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
        }

    }

}