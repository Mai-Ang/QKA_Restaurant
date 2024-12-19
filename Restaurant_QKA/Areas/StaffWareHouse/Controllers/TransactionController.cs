using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class TransactionController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/Transaction
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            ViewBag.Staff = db.PersonnelFiles.ToList();
            ViewBag.Material = db.WareHouses.ToList();
            List<InventoryTransaction> transactions = db.InventoryTransactions.OrderByDescending(t => t.TransactionDate).ToList();
            return View(transactions);
        }
        public ActionResult Search(string query)
        {
            ViewBag.Query = query;
            ViewBag.Staff = db.PersonnelFiles.ToList();
            ViewBag.Material = db.WareHouses.ToList();
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<InventoryTransaction>());
            }
            var idmaterial = db.WareHouses.FirstOrDefault(m => m.Name.Contains(query));
            var results = db.InventoryTransactions.Where(p => p.MaterialID == idmaterial.MaterialID);
            return View(results);
        }
        // Thêm mới nhà cung cấp
        public ActionResult Create(int? idsupplier)
        {
            ViewBag.Material = db.WareHouses.Where( m => m.IsActive == true).ToList();
            if(idsupplier != null)
            {
                ViewBag.Supplier = db.Suppliers.FirstOrDefault(sup => sup.SupplierID == idsupplier);
            }
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateTransaction.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryTransaction it)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            if (ModelState.IsValid)
            {
                it.StaffID = (int)Session["UserID"];
                it.TotalPrice = it.TotalPrice * it.Quantity;
                it.TransactionDate = DateTime.Now;
                db.InventoryTransactions.Add(it);
                db.SaveChanges();

                var existmaterial = db.WareHouses.FirstOrDefault(m => m.MaterialID == it.MaterialID);
                if (existmaterial != null)
                {
                    existmaterial.Quantity = existmaterial.Quantity + it.Quantity;
                    existmaterial.ImportPrice = it.TotalPrice / it.Quantity;
                    db.Entry(existmaterial).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            // Nếu xảy ra lỗi vẫn load lại danh sách 
            ViewBag.Material = db.WareHouses.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateTransaction.cshtml", it);
        }
    }
}