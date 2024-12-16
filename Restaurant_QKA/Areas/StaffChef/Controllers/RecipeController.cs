using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;  // Đảm bảo sử dụng đúng namespace của Entity Framework

namespace Restaurant_QKA.Areas.StaffChef.Controllers
{
    public class RecipeController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();

        // Hiển thị danh sách công thức món ăn
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var menuItems = db.MenuItems
                .Include(m => m.Recipes) // Lấy công thức của món ăn
                .Include(m => m.Recipes.Select(r => r.WareHouse)) // Lấy thông tin kho nguyên liệu của công thức
                .ToList();

            return View(menuItems);
        }












        // Tạo mới công thức món ăn
        public ActionResult Create(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var menuItem = db.MenuItems.Include(m => m.Recipes)
                                       .FirstOrDefault(m => m.ItemID == id);
            var warehouseItems = db.WareHouses.ToList(); // Lấy danh sách nguyên liệu từ kho

            if (menuItem == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách nguyên liệu đã được chọn cho món ăn này
            var usedMaterialIds = menuItem.Recipes.Select(r => r.MaterialID).ToList();

            // Truyền danh sách nguyên liệu và món ăn vào view
            ViewBag.MenuItemName = menuItem.Name;
            ViewBag.MenuItemId = menuItem.ItemID;
            ViewBag.UsedMaterials = usedMaterialIds;

            return View(warehouseItems); // Truyền nguyên liệu vào view
        }

        [HttpPost]
        public ActionResult Create(int ItemID, int MaterialID, float Quantity)
        {
            var recipe = new Recipe
            {
                ItemID = ItemID,
                MaterialID = MaterialID,
                Quantity = Quantity,
                CreatedStaff = 1, // Bạn có thể lấy giá trị này từ session hoặc tài khoản đang đăng nhập
            };

            db.Recipes.Add(recipe); // Thêm công thức mới vào cơ sở dữ liệu
            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            return RedirectToAction("Index", "Recipe"); // Quay lại danh sách công thức
        }







        
        // Xóa công thức món ăn
        public ActionResult Delete(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}