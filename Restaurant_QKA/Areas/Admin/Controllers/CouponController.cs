using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.Admin.Controllers
{
    public class CouponController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: Admin/Coupon
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            List<Coupon> coupon = db.Coupons.ToList();
            ViewBag.Manager = db.PersonnelFiles.Find((int)Session["UserID"]);
            return View(coupon);
        }
        // Tìm kiếm Coupon 
        public ActionResult Search(string query)
        {
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Coupon>());
            }
            var results = db.Coupons.Where(u => u.Name.Contains(query));
            return View(results);
        }

        // Thêm mới sản phẩm 
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Shared/_CreateCoupont.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coupon Coupon)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

                Coupon.CreatedDate = DateTime.Now;
                Coupon.CreatedManagerID = (int)Session["UserID"];
                db.Coupons.Add(Coupon);
                db.SaveChanges();
                return Json(new { success = true });

            }
            return PartialView("~/Areas/Admin/Views/Shared/_CreateCoupont.cshtml", Coupon);
        }

        // Điều chỉnh Coupon
        // Get Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon Coupon = db.Coupons.Find(id);
            if (Coupon == null)
            {
                return HttpNotFound();
            };
            return PartialView("~/Areas/Admin/Views/Shared/_EditCoupont.cshtml", Coupon);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Coupon Coupon)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm coupon cần chỉnh sửa trong cơ sở dữ liệu
                    var existCoupon = db.Coupons.Find(Coupon.CouponID);

                    if (existCoupon == null)
                    {
                        return Json(new { success = false });
                    }

                    // Cập nhật thông tin coupon
                    existCoupon.Name = Coupon.Name;
                    existCoupon.Value = Coupon.Value;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existCoupon).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                return PartialView("~/Areas/Admin/Views/Shared/_EditCoupont.cshtml", Coupon);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }
        // Xóa Coupon
        // Get Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon Coupon = db.Coupons.Find(id);
            if (Coupon == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Areas/Admin/Views/Shared/_DeleteCoupont.cshtml", Coupon);
        }

        // Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Coupon Coupon = db.Coupons.Find(id);

                if (Coupon == null)
                {
                    // Trả về phản hồi JSON cho trường hợp Coupon không tồn tại
                    return Json(new { success = false });
                }

                db.Coupons.Remove(Coupon);
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