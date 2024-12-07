using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        // GET: Admin/Staff
        Restaurant_Entities db = new Restaurant_Entities();
        public ActionResult Index()
        {
            List<Staff> liststaffs = db.Staffs.ToList();
            return View(liststaffs);
        }
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Shared/_CreateCoupont.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coupon Coupon)
        {
            if (ModelState.IsValid)
            {
                Coupon.CreatedDate = DateTime.Now;
                db.Coupons.Add(Coupon);
                db.SaveChanges();
                return Json(new { success = true });

            }
            return PartialView("~/Areas/Admin/Views/Shared/_CreateCoupont.cshtml", Coupon);
        }
    }
}