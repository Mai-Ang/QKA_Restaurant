using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class OrderHistoryController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();

        // Action để hiển thị lịch sử đơn hàng của khách hàng hiện tại
        public ActionResult Index()
        {
            // Lấy UserID từ session, giả sử là integer
            var userId = Session["UserID"] as int?;

            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });

            // Lấy danh sách đơn hàng của khách hàng hiện tại từ database
            var orders = db.Orders.Where(o => o.CusID == userId).ToList();

            // Phân loại đơn hàng theo trạng thái
            var pendingOrders = orders.Where(o => o.Status == "0").ToList(); // Chờ xác nhận
            var shippingOrders = orders.Where(o => o.Status == "1").ToList(); // Chờ giao
            var deliveredOrders = orders.Where(o => o.Status == "2").ToList(); // Đã giao
            var canceledOrders = orders.Where(o => o.Status == "3").ToList(); // Đã hủy

            // Truyền dữ liệu xuống View
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.ShippingOrders = shippingOrders;
            ViewBag.DeliveredOrders = deliveredOrders;
            ViewBag.CanceledOrders = canceledOrders;

            return View();
        }

        // Action xác nhận đã nhận hàng
        [HttpPost]
        public ActionResult ConfirmReceived(int orderId)
        {
            // Lấy UserID từ session, giả sử là integer
            var userId = Session["UserID"] as int?;

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy đơn hàng từ database, chỉ lấy của người dùng hiện tại
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId && o.CusID == userId);

            if (order != null)
            {
                // Cập nhật trạng thái đơn hàng thành "Đã giao"
                order.Status = "2"; // "2" là trạng thái đã giao

                // Cập nhật ngày giao
                order.DeliveryDate = DateTime.Now;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    // Xử lý lỗi (ví dụ ghi log)
                    ViewBag.ErrorMessage = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng.";
                }
            }

            return RedirectToAction("Index");
        }

        // Action hủy đơn
        [HttpPost]
        public ActionResult CancelOrder(int orderId)
        {
            // Lấy UserID từ session, giả sử là integer
            var userId = Session["UserID"] as int?;

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy đơn hàng từ database, chỉ lấy của người dùng hiện tại
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId && o.CusID == userId);

            if (order != null)
            {
                // Cập nhật trạng thái đơn hàng thành "Đã hủy"
                order.Status = "3"; // "3" là trạng thái đã hủy

                // Cập nhật ngày hủy
                order.DeliveryDate = DateTime.Now;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    // Xử lý lỗi (ví dụ ghi log)
                    ViewBag.ErrorMessage = "Có lỗi xảy ra khi hủy đơn hàng.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}
