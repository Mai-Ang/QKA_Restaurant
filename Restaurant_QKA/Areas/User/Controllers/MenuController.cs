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
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: User/Home
        public ActionResult Index()
        {
            var iduser = Session["UserID"];
            ViewBag.TotalItemsInCart = GetTotalItemsInCart((int?)iduser);
            var menuItems = db.MenuItems.Where(m => m.IsActive ?? false).ToList();
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



        [HttpPost]
        public ActionResult AddComment(int itemId, string content)
        {
            // Retrieve user ID from Session
            var iduser = Session["UserID"];
            if (iduser == null)
                return RedirectToAction("Login", "User");

            // Create and save the comment
            var comment = new Comment
            {
                CusID = (int)iduser, // Cast iduser to int
                ItemID = itemId,
                Content = content,
                CreatedDate = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            // Redirect back to the item details page
            return RedirectToAction("DetailMenuItem", "Menu", new { id = itemId });
        }




    }
}
