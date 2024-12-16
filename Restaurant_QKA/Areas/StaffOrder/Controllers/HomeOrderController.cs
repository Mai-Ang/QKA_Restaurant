using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffOrder.Controllers
{
    public class HomeOrderController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu
            var orders = db.Orders.ToList();

            // Phân chia đơn hàng theo trạng thái
            ViewBag.PendingOrders = orders.Where(o => o.Status == "0").ToList();
            ViewBag.ShippingOrders = orders.Where(o => o.Status == "1").ToList();
            ViewBag.DeliveredOrders = orders.Where(o => o.Status == "2").ToList();
            ViewBag.CanceledOrders = orders.Where(o => o.Status == "3").ToList();

            return View();
        }


        [HttpPost]
        public ActionResult CancelOrder(int orderId)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order != null)
            {
                order.Status = "3"; // "3" là trạng thái đã hủy
                order.DeliveryDate = DateTime.Now;

                try
                {
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công!";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy đơn hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng để hủy.";
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        public ActionResult ConfirmReceived(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order != null)
            {
                order.Status = "1"; 
                order.DeliveryDate = DateTime.Now;

                try
                {
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đơn hàng đã được xác nhận là đã nhận.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng.";
                }
            }

            return RedirectToAction("Index");
        }

    }
}