using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            ViewBag.CountProducts = db.MenuItems.Count();
            ViewBag.CountTotalOrders = db.Orders.Count();
            ViewBag.CountCus = db.Customers.Count();
            ViewBag.CountSuccessOrders = db.Orders.Count(m => m.Status == "1");

            ViewBag.Revenue = db.Invoices.Sum(m => m.Total);
            List<Invoice> invoices = db.Invoices.ToList();
            return View(invoices);
        }
    }
}