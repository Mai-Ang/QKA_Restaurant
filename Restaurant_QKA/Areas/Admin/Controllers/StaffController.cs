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
    public class StaffController : Controller
    {
        // GET: Admin/Staff
        Restaurant_Entities db = new Restaurant_Entities();
        // HashPass
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
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            ViewBag.Staff = db.PersonnelFiles.ToList();
            List<Staff> liststaffs = db.Staffs.ToList();
            return View(liststaffs);
        }
        // Tìm kiếm Staff 
        public ActionResult Search(string query)
        {
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Staff>());
            }
            var results = db.PersonnelFiles.Where(per => per.Name.Contains(query));
            return View(results);
        }
        public ActionResult Create()
        {
            ViewBag.Position = db.Positions.ToList();
            ViewBag.BasicSalary = db.BasicSalaries.ToList();
            return PartialView("~/Areas/Admin/Views/Shared/_CreateStaff.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                // Create Staff Account
                var basicsalary = db.BasicSalaries.Where(salary => salary.BasicSalaryID == staff.BasicSalaryID)
                                                  .Select(salary => salary.BasicSalary1)
                                                  .SingleOrDefault();
                var salarygrading = db.Positions.Where(sg => sg.PositionID == staff.PositionID)
                                                  .Select(sg => sg.SalaryGrading)
                                                  .SingleOrDefault(); ;
                staff.Salary = basicsalary * salarygrading;
                staff.Password = HashPassword(staff.Password);
                db.Staffs.Add(staff);
                db.SaveChanges();

                // Create PersonelFile
                var personel = new PersonnelFile
                {
                    StaffID = staff.StaffID,
                    StartingDate = DateTime.Now,
                };
                db.PersonnelFiles.Add(personel);
                db.SaveChanges();

                // Add Role
                if(staff.Role == "Bep")
                {
                    var newstaffchef = new Models.StaffChef
                    {
                        StaffID = staff.StaffID,
                    };
                    db.StaffChefs.Add(newstaffchef);
                    db.SaveChanges();
                }
                else if (staff.Role == "QuanLy")
                {
                    var newmanager = new Models.Manager
                    {
                        ManagerID = staff.StaffID,
                    };
                    db.Managers.Add(newmanager);
                    db.SaveChanges();
                }
                else if (staff.Role == "Kho")
                {
                    var newstaffwh = new Models.StaffWareHouse
                    {
                        StaffID = staff.StaffID,
                    };
                    db.StaffWareHouses.Add(newstaffwh);
                    db.SaveChanges();
                }
                else if (staff.Role == "KinhDoanh")
                {
                    var newstafforder = new Models.StaffOrder
                    {
                        StaffID = staff.StaffID,
                    };
                    db.StaffOrders.Add(newstafforder);
                    db.SaveChanges();
                }
                return Json(new { success = true });

            }
            ViewBag.Position = db.Positions.ToList();
            ViewBag.BasicSalary = db.BasicSalaries.ToList();
            return PartialView("~/Areas/Admin/Views/Shared/_CreateStaff.cshtml", staff);
        }
        
        // Xem thông tin chi tiết nhân viên 
        public ActionResult Detail(int? id)
        {
            var existprofile = db.PersonnelFiles.Find(id);
            var existstaff = db.Staffs.Find(existprofile.StaffID);
            ViewBag.CurrentPosition = db.Positions.Find(existstaff.PositionID);
            ViewBag.CurrentRole = existstaff.Role;
            return View(existprofile);
        }
        
        // Điều chỉnh thông tin cá nhân nhân viên
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonnelFile personnel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existpersonelfile = db.PersonnelFiles.Find(personnel.StaffID);
                    existpersonelfile.Name = personnel.Name;
                    existpersonelfile.Phone = personnel.Phone;
                    existpersonelfile.Email = personnel.Email;
                    existpersonelfile.DateOfBirth = personnel.DateOfBirth;
                    existpersonelfile.Address = personnel.Address;
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existpersonelfile).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                return View(personnel);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }
        
        // Điều Chỉnh Lương 
        // Get Method
        public ActionResult ChangeSalary(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            };
            ViewBag.CurrentName = db.PersonnelFiles.Find(id);
            ViewBag.CurrentGrading = db.Positions.Find(staff.PositionID);
            ViewBag.Salary = db.BasicSalaries.Find(staff.BasicSalaryID);

            ViewBag.BasicSalary = db.BasicSalaries.ToList();
            ViewBag.Position = db.Positions.ToList();
            return PartialView("~/Areas/Admin/Views/Shared/_ChangeSalary.cshtml", staff);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSalary(Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existstaff = db.Staffs.Find(staff.StaffID);
                    existstaff.PositionID = staff.PositionID;
                    existstaff.BasicSalaryID = staff.BasicSalaryID;
                    
                    var salary = db.BasicSalaries.FirstOrDefault(s => s.BasicSalaryID == staff.BasicSalaryID);
                    var grading = db.Positions.FirstOrDefault(s => s.PositionID == staff.PositionID);

                    existstaff.Salary = salary.BasicSalary1 * grading.SalaryGrading;
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existstaff).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                ViewBag.BasicSalary = db.BasicSalaries.ToList();
                ViewBag.Position = db.Positions.ToList();
                return PartialView("~/Areas/Admin/Views/Shared/_ChangeSalary.cshtml", staff);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }
    }
}