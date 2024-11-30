using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;  // Đảm bảo bạn có thêm namespace này để sử dụng Include


namespace Restaurant_QKA.Areas.User.Controllers
{
    public class MenuController : Controller
    {
        RestaurantEntities db = new RestaurantEntities();
        // GET: User/Home
        public ActionResult Index()
        {
            var menuItems = db.MenuItems.Where(m => m.IsActive ?? false).ToList();
            return View(menuItems);
        }


        public ActionResult DetailMenuItem(int id)
        {
            var menuItem = db.MenuItems
                .Include(m => m.Comments.Select(c => c.Customer)) // Nạp Comments và Customer
                .FirstOrDefault(m => m.ItemID == id);

            if (menuItem == null)
            {
                return HttpNotFound();
            }

            return View(menuItem);
        }
    }
}