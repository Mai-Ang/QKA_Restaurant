using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class SupplierController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/Supplier
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            List<Supplier> suppliers = db.Suppliers.ToList();
            return View(suppliers);
        }
        public ActionResult Search(string query)
        {
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Supplier>());
            }
            var results = db.Suppliers.Where(p => p.Name.Contains(query));
            return View(results);
        }
        // Thêm mới nhà cung cấp
        public ActionResult Create()
        {
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateSupplier.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Supplier supplier)
        {

            if (ModelState.IsValid)
            {
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return Json(new { success = true });
            }
            // Nếu xảy ra lỗi vẫn load lại danh sách 
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateSupplier.cshtml", supplier);
        }
        // Điều chỉnh nhà cung cấp
        // Get Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            };
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_EditSupplier.cshtml", supplier);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm nhà cung cấpcần chỉnh sửa trong cơ sở dữ liệu
                    var existsupplier = db.Suppliers.Find(supplier.SupplierID);

                    if (existsupplier == null)
                    {
                        return Json(new { success = false });
                    }

                    // Cập nhật thông tin sản phẩm
                    existsupplier.Name = supplier.Name;
                    existsupplier.Address = supplier.Address;
                    existsupplier.Phone = supplier.Phone;
                    existsupplier.Email = supplier.Email;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existsupplier).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                ViewBag.Supplier = db.Suppliers.ToList();
                return PartialView("~/Areas/StaffWareHouse/Views/Shared/_EditSupplier.cshtml", supplier);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }

        // Xóa nhà cung cấp
        // Get Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_DeleteSupplier.cshtml", supplier);
        }

        // Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Supplier supplier = db.Suppliers.Find(id);

                if (supplier == null)
                {
                    // Trả về phản hồi JSON cho trường hợp nhà cung cấp không tồn tại
                    return Json(new { success = false });
                }

                db.Suppliers.Remove(supplier);
                db.SaveChanges();

                // Xóa thành công, trả về phản hồi JSON
                return Json(new { success = true });
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }
    }
}