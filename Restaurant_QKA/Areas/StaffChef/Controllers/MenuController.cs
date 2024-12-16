using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffChef.Controllers
{
    public class MenuController : Controller
    {
        // GET: StaffChef/Menu
        Restaurant_Entities db = new Restaurant_Entities();
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var menuItems = db.MenuItems.Include("Category").ToList();
            var categories = db.Categories.ToList();

            ViewBag.Categories = categories; // Đưa danh sách loại món vào ViewBag
            return View(menuItems);
        }





        [HttpPost]
        public ActionResult IsActive(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var item = db.MenuItems.Find(id); // Tìm món ăn dựa trên ID
            if (item != null)
            {
                // Đảo ngược trạng thái
                item.IsActive = !item.IsActive;

                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Trả về trạng thái mới
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false });
        }

    }
}