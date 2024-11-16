using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class UserController : Controller
    {
        RestaurantEntities db = new RestaurantEntities();
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
        // *********** REGISTER ***********
        // GET: User/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer cus)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tài khoản đã tồn tại chưa
                var existingUser = db.Customers.FirstOrDefault(x => x.UserName == cus.UserName || x.Email == cus.Email);
                if (existingUser != null)
                {
                    ViewBag.Mess = "Tài khoản hoặc email đã được sử dụng.";
                    return View(cus);
                }
                cus.HashPass = HashPassword(cus.HashPass);

                // Thêm mới khách hàng vào cơ sở dữ liệu
                db.Customers.Add(cus);
                db.SaveChanges();
                ViewBag.Mess = "Đăng ký thành công";
                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "User");
            }
            return View(cus);
        }
        //*********** LOGIN ***********
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string matkhau)
        {
            string hashPass = HashPassword(matkhau);
            var staff = db.Staffs.FirstOrDefault(x => x.UserName == email && x.Password == hashPass);
            if (staff != null)
            {
                // Kiểm tra quyền admin
                if (db.Managers.FirstOrDefault(x => x.ManagerID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.UserID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                }
                // Kiểm tra quyền đầu bếp
                else if (db.StaffChefs.FirstOrDefault(x => x.StaffID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.UserID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                }
                // Kiểm tra quyền nhân viên order
                else if (db.StaffOrders.FirstOrDefault(x => x.StaffID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.UserID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                }
                // Kiểm tra quyền nhân viên kho
                else if (db.StaffWareHouses.FirstOrDefault(x => x.StaffID != staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.UserID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                }
                return View();
            }
            else
            {
                var user = db.Customers
                .FirstOrDefault(kh => kh.Email == email && kh.HashPass == hashPass);

                if (user != null)
                {
                    // Đăng nhập thành công, có thể lưu thông tin người dùng vào Session
                    ViewBag.SuccessLogin = true;
                    Session["UserID"] = user.CusID;
                    Session["UserName"] = user.Name;
                    return View(); // Điều hướng đến trang chính hoặc trang khác
                }
                else
                {
                    // Đăng nhập không thành công
                    ViewBag.Mess = "Tài khoản hoặc mật khẩu không đúng.";
                    return View();
                }
            }
        }

        public ActionResult InforUser()
        {
            if (Session["UserName"] != null)
            {
                // Lấy tên người dùng từ Session
                string userName = Session["UserName"].ToString();
                // Lấy thông tin người dùng từ cơ sở dữ liệu (giả sử bạn có hàm để làm việc này)
                var user = db.Customers.FirstOrDefault(x => x.UserName == userName);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}