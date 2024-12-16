using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        //HashPass SHA-256
        private string HashPassword(string password)
        {
            // Chuyển đổi mật khẩu sang mảng byte
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Tạo đối tượng SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Mã hóa mật khẩu
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Chuyển đổi mảng byte đã mã hóa thành chuỗi hex
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển đổi thành chuỗi hex
                }
                return sb.ToString();
            }
        }
        // GET: Admin/Customer
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            List<Customer> cus = db.Customers.ToList();
            return View(cus);
        }

        // Tìm kiếm khách hàng 
        public ActionResult Search(string query)
        {
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Customer>());
            }
            var results = db.Customers.Where(u => u.Name.Contains(query));
            return View(results);
        }

        // Điều chỉnh thông tin mật khẩu
        // Get Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer cus = db.Customers.Find(id);
            if (cus == null)
            {
                return HttpNotFound();
            };
            return PartialView("~/Areas/Admin/Views/Shared/_EditPassUser.cshtml", cus);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm sản phẩm cần chỉnh sửa trong cơ sở dữ liệu
                    var existuser = db.Customers.Find(customer.CusID);

                    if (existuser == null)
                    {
                        return Json(new { success = false });
                    }

                    // Kiểm tra nếu người dùng nhập mật khẩu mới
                    if (!string.IsNullOrEmpty(customer.HashPass))
                    {
                        // Chỉ cập nhật HashPass khi mật khẩu mới được nhập
                        existuser.HashPass = HashPassword(customer.HashPass);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existuser).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                ViewBag.Category = db.Categories.ToList();
                return PartialView("~/Areas/Admin/Views/Shared/_EditPassUser.cshtml", customer);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }


        // Xóa
        // Get Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer cus = db.Customers.Find(id);
            if (cus == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Areas/Admin/Views/Shared/_DeleteUser.cshtml", cus);
        }

        // Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Customer cus = db.Customers.Find(id);

                if (cus == null)
                {
                    // Trả về phản hồi JSON cho trường hợp sản phẩm không tồn tại
                    return Json(new { success = false });
                }

                db.Customers.Remove(cus);
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