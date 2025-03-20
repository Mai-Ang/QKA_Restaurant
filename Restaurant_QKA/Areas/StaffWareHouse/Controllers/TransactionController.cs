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
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "User", new { area = "User" });

            if (ModelState.IsValid)
            {
                // Sử dụng transaction
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Gán thông tin cần thiết và thêm giao dịch nhập kho
                        it.StaffID = (int)Session["UserID"];
                        it.TotalPrice = it.TotalPrice * it.Quantity;
                        it.TransactionDate = DateTime.Now;
                        db.InventoryTransactions.Add(it);
                        db.SaveChanges();

                        // 2. Kiểm tra sản phẩm có tồn tại hay không và cập nhật kho
                        var existmaterial = db.WareHouses.FirstOrDefault(m => m.MaterialID == it.MaterialID);
                        if (existmaterial != null)
                        {
                            existmaterial.Quantity += it.Quantity;
                            existmaterial.ImportPrice = it.TotalPrice / it.Quantity;

                            db.Entry(existmaterial).State = EntityState.Modified;
                            db.SaveChanges();

                            // 3. Commit transaction nếu không có lỗi
                            transaction.Commit();
                            return Json(new { success = true });
                        }
                        else
                        {
                            // Rollback transaction nếu sản phẩm không tồn tại
                            transaction.Rollback();
                            return Json(new { success = false, message = "Sản phẩm không tồn tại trong kho!" });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi xảy ra
                        transaction.Rollback();
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }

            // Nếu có lỗi, vẫn load lại danh sách vật tư và nhà cung cấp
            ViewBag.Material = db.WareHouses.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateTransaction.cshtml", it);
        }

    }
}