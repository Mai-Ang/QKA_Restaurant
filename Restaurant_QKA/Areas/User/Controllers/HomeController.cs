using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: User/Home
        public ActionResult Index()
        {
            var iduser = Session["UserID"];
            var menuItems = db.MenuItems.Where(m => m.IsActive ?? false).Take(8).ToList();
            ViewBag.TotalItemsInCart = GetTotalItemsInCart((int?)iduser);

            ViewBag.Totalcus = db.Customers.Count();
            ViewBag.Totalstaffs = db.Staffs.Count();
            ViewBag.Totalitems = db.MenuItems.Count();
            return View(menuItems);
        }


        private int GetTotalItemsInCart(int? userId)
        {
            if (userId == null) return 0;

            // Lấy CartID dựa trên UserID
            var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.CusID == userId);
            if (cart == null) return 0;

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            int totalItems = cart.CartItems.Count();
            return totalItems;
        }

        public ActionResult Contact()
        {
            var iduser = Session["UserID"];

            ViewBag.TotalItemsInCart = GetTotalItemsInCart((int?)iduser);
            return View();
        }
    }
}
