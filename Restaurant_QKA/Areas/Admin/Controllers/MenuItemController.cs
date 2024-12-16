using PagedList;
using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.Admin.Controllers
{
    public class MenuItemController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // Hiển thị danh sách tất cả sản phẩm
        public ActionResult Index(int? page)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            int pagesize = 5;
            int pagenumber = (page ?? 1);
            ViewBag.Categories = db.Categories.ToList();

            IPagedList<MenuItem> products = db.MenuItems.OrderBy(p => p.CategoryID).ToPagedList(pagenumber, pagesize);
            return View(products);
        }

        // Lọc sản phẩm theo danh mục
        public ActionResult ProductsByCategory(int? page, int categoryId)
        {
            int pagesize = 5;
            int pagenumber = (page ?? 1);
            ViewBag.CurrentId = categoryId;
            ViewBag.CurrentCate = db.Categories.Find(categoryId);
            ViewBag.Categories = db.Categories.ToList();
            //ViewBag.Category = db.Categories.Select(p => p.Name).Where(p => p.Id).ToList();

            var productswcategory = db.MenuItems.Where(p => p.CategoryID == categoryId).OrderBy(p => p.CategoryID).ToPagedList(pagenumber, pagesize);
            return View(productswcategory);
        }

        // Tìm kiếm sản phẩm 
        public ActionResult Search(string query)
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Query = query;

            if (string.IsNullOrEmpty(query))
            {
                return View(new List<MenuItem>());
            }
            var results = db.MenuItems.Where(p => p.Name.Contains(query));
            return View(results);
        }

        // Thêm mới sản phẩm 
        public ActionResult Create()
        {
            ViewBag.Category = db.Categories.ToList();

            return PartialView("~/Areas/Admin/Views/Shared/_CreateProduct.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuItem product, HttpPostedFileBase ImageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".png" };
            var fileExtension = Path.GetExtension(ImageFile.FileName)?.ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Json(new { success = false });
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Đặt tên file duy nhất GUID
                    var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    var extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + Guid.NewGuid() + extension;

                    // Đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Content/MenuItems"), fileName);

                    // Lưu file vào thư mục 
                    ImageFile.SaveAs(path);

                    product.CreatedDate = DateTime.Now;
                    // Gán đường dẫn file vào thuộc tính ImageUrl của Product
                    product.ImageUrl = fileName;
                }

                db.MenuItems.Add(product);
                db.SaveChanges();
                return Json(new { success = true });

            }
            // Nếu xảy ra lỗi vẫn load lại danh sách category 
            ViewBag.Category = db.Categories.ToList();
            return PartialView("~/Areas/Admin/Views/Shared/_CreateProduct.cshtm.cshtml", product);
        }

        // Điều chỉnh sản phẩm 
        // Get Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem product = db.MenuItems.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            };
            ViewBag.Category = db.Categories.ToList();
            return PartialView("~/Areas/Admin/Views/Shared/_EditProduct.cshtml", product);
        }
        // Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MenuItem product, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm sản phẩm cần chỉnh sửa trong cơ sở dữ liệu
                    var existproduct = db.MenuItems.Find(product.ItemID);

                    if (existproduct == null)
                    {
                        return Json(new { success = false });
                    }

                    // Cập nhật thông tin sản phẩm
                    existproduct.Name = product.Name;
                    existproduct.Description = product.Description;
                    existproduct.Price = product.Price;
                    existproduct.Quantity = product.Quantity;
                    existproduct.IsActive = product.IsActive;
                    existproduct.CategoryID = product.CategoryID;

                    // Kiểm tra xem người dùng có upload ảnh mới hay không
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        var oldImagePath = Server.MapPath("~/Content/Images/Products_Img/" + existproduct.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        // Đặt tên file duy nhất GUID
                        var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        var extension = Path.GetExtension(ImageFile.FileName);
                        fileName = fileName + "_" + Guid.NewGuid() + extension;

                        // Đường dẫn lưu file
                        var path = Path.Combine(Server.MapPath("~/Content/Images/Products_Img"), fileName);

                        // Lưu file vào thư mục
                        ImageFile.SaveAs(path);

                        // Gán đường dẫn file vào thuộc tính ImageUrl của Product
                        product.ImageUrl = fileName;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Entry(existproduct).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                // Nếu có lỗi validation
                ViewBag.Category = db.Categories.ToList();
                return PartialView("~/Areas/Admin/Views/Shared_EditProduct.cshtml", product);
            }
            catch (Exception)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }

        // Xóa sản phẩm 
        // Get Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem product = db.MenuItems.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Areas/Admin/Views/Shared/_DeleteProduct.cshtml", product);
        }

        // Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MenuItem product = db.MenuItems.Find(id);

                if (product == null)
                {
                    // Trả về phản hồi JSON cho trường hợp sản phẩm không tồn tại
                    return Json(new { success = false });
                }

                db.MenuItems.Remove(product);
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