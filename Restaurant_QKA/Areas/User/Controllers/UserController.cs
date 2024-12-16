using Restaurant_QKA.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class UserController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
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
                ViewBag.Success = true;
                // Chuyển hướng đến trang đăng nhập
                return View();
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
        public ActionResult Login(string email, string hashpass)
        {
            string hashPass = HashPassword(hashpass);
            var staff = db.Staffs.FirstOrDefault(x => x.UserName == email && x.Password == hashPass);
            if (staff != null)
            {
                // Kiểm tra quyền admin
                if (db.Managers.FirstOrDefault(x => x.ManagerID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.StaffID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    Session["Img"] = staffname.ImageUrl;
                    Session["Email"] = staffname.Email;

                    return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                }
                // Kiểm tra quyền đầu bếp
                else if (db.StaffChefs.FirstOrDefault(x => x.StaffID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.StaffID == staff.StaffID);
                    Session["Img"] = staffname.ImageUrl;
                    Session["UserName"] = staffname.Name;
                    Session["Email"] = staffname.Email;

                    return RedirectToAction("Index", "Menu", new { Area = "StaffChef" });
                }
                // Kiểm tra quyền nhân viên order
                else if (db.StaffOrders.FirstOrDefault(x => x.StaffID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.StaffID == staff.StaffID);
                    Session["UserName"] = staffname.Name;
                    Session["Img"] = staffname.ImageUrl;
                    Session["Email"] = staffname.Email;

                    return RedirectToAction("Index", "HomeOrder", new { Area = "StaffOrder" });
                }
                // Kiểm tra quyền nhân viên kho
                else if (db.StaffWareHouses.FirstOrDefault(x => x.StaffID == staff.StaffID) != null)
                {
                    Session["UserID"] = staff.StaffID;
                    var staffname = db.PersonnelFiles.FirstOrDefault(x => x.StaffID == staff.StaffID);
                    Session["UserName"] = staffname.Name; 
                    Session["Img"] = staffname.ImageUrl;
                    Session["Email"] = staffname.Email;

                    return RedirectToAction("Index", "HomeWareHouse", new { Area = "StaffWareHouse" });
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
                    ViewBag.Success = true;
                    Session["UserID"] = user.CusID;
                    Session["UserName"] = user.Name;
                    Session["Img"] = user.ImageUrl;

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

        //*********** User Profile ***********
        public ActionResult UserProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["UserID"];
            Customer user = db.Customers.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        //*********** Update Profile ***********
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Customer cus)
        {
            if (ModelState.IsValid)
            {
                var existuser = db.Customers.Find(cus.CusID);
                if (cus != null)
                {
                    existuser.Name = cus.Name;
                    existuser.Email = cus.Email;
                    existuser.Phone = cus.Phone;
                    existuser.DateOfBirth = cus.DateOfBirth;
                    existuser.Address = cus.Address;

                    try
                    {
                        db.Entry(existuser).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                        TempData["ErrorMessage"] = null;
                        return RedirectToAction("UserProfile");
                    }
                    catch (Exception)
                    {
                        TempData["SuccessMessage"] = null;
                        TempData["ErrorMessage"] = "Cập nhật thông tin không thành công!";
                    }
                }
            }
            return View("UserProfile", cus);
        }
        //*********** Change Password ***********
        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string email, string oldpass, string newpass)
        {
            if (Session["UserID"] == null) return Redirect("Login");
            if (ModelState.IsValid)
            {
                var existuser = db.Customers.Find(Session["UserID"]);
                if (existuser != null)
                {
                    if(existuser.Email == email && existuser.HashPass == HashPassword(oldpass))
                    {
                        existuser.HashPass = HashPassword(newpass);
                        db.Entry(existuser).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Success = true;
                        return View();
                    }
                }
            }
            else
            {
                ViewBag.Success = false;
            }
            return View();
        }

        //*********** Logout ***********
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}