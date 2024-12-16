using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class HomeWareHouseController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/HomeWareHouse
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            List<WareHouse> listwarehouse = db.WareHouses.ToList();
            ViewBag.Suppliers = db.Suppliers.ToList();
            return View(listwarehouse);
        }

        public ActionResult Search(string query)
        {
            ViewBag.Suppliers = db.Suppliers.ToList();
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<WareHouse>());
            }
            var results = db.WareHouses.Where(p => p.Name.Contains(query));
            return View(results);
        }
        // Thêm mới nguyên liệu
        public ActionResult Create()
        {
            ViewBag.Supplier = db.Suppliers.ToList();

            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateMaterial.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WareHouse material)
        {

            if (ModelState.IsValid)
            {
                material.Quantity = 0;
                material.ImportPrice = 0;
                db.WareHouses.Add(material);
                db.SaveChanges();
                return Json(new { success = true });
            }
            // Nếu xảy ra lỗi vẫn load lại danh sách 
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_CreateMaterial.cshtml", material);
        }
        // Điều chỉnh nguyên liệu
        // Get Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse material = db.WareHouses.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            };
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_EditMaterial.cshtml", material);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WareHouse material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm nguyên liệucần chỉnh sửa trong cơ sở dữ liệu
                    var existmaterial = db.WareHouses.Find(material.MaterialID);

                    if (existmaterial == null)
                    {
                        return Json(new { success = false });
                    }

                    // Cập nhật thông tin sản phẩm
                    existmaterial.Name = material.Name;
                    existmaterial.IsActive = material.IsActive;
                    existmaterial.SupplierID = material.SupplierID;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existmaterial).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                ViewBag.Supplier = db.Suppliers.ToList();
                return PartialView("~/Areas/StaffWareHouse/Views/Shared/_EditMaterial.cshtml", material);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }

        // Xóa nguyên liệu
        // Get Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse material = db.WareHouses.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier = db.Suppliers.ToList();
            return PartialView("~/Areas/StaffWareHouse/Views/Shared/_DeleteMaterial.cshtml", material);
        }

        // Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                WareHouse material = db.WareHouses.Find(id);

                if (material == null)
                {
                    // Trả về phản hồi JSON cho trường hợp nguyên liệu không tồn tại
                    return Json(new { success = false });
                }

                db.WareHouses.Remove(material);
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