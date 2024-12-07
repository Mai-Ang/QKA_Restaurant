using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class CartsController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();

        public ActionResult AddToCart(int menuItemId, int quantity)
        {
            // Lấy iduser từ Session
            var iduser = Session["UserID"];
            if (iduser == null)
                return RedirectToAction("Login", "User");

            var cart = GetCart((int)iduser);
            var menuItem = db.MenuItems.Find(menuItemId);

            if (menuItem != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MenuItemID == menuItemId);
                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        MenuItemID = menuItemId,
                        Quantity = quantity,
                        Price = menuItem.Price,
                        Total = menuItem.Price * quantity,
                        CreatedDate = DateTime.Now
                    };
                    cart.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += quantity;
                    cartItem.Total = cartItem.Quantity * menuItem.Price;
                }

                db.SaveChanges();
            }

            // Quay lại trang trước đó
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());

            // Nếu không có UrlReferrer, quay lại trang chủ
            return RedirectToAction("Index", "Home");
        }



        // Cập nhật số lượng sản phẩm trong giỏ
        [HttpPost]
        public JsonResult UpdateCart(int cartItemId, int quantity)
        {
            var cartItem = db.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                // Cập nhật số lượng và tổng tiền của dòng
                cartItem.Quantity = quantity;
                cartItem.Total = cartItem.Quantity * cartItem.Price;
                db.SaveChanges();

                // Tính tổng cộng mới của giỏ hàng
                var cart = db.Carts.Include(c => c.CartItems)
                                   .FirstOrDefault(c => c.CartID == cartItem.CartID);
                var newCartTotal = cart.CartItems.Sum(ci => ci.Total);

                return Json(new { success = true, updatedTotal = string.Format("{0:C}", cartItem.Total), newCartTotal = string.Format("{0:C}", newCartTotal) });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
        }



        // Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public ActionResult RemoveFromCart(int cartItemId)
        {
            // Lấy iduser từ Session
            var iduser = Session["UserID"];
            if (iduser == null)
                return RedirectToAction("Login", "User");

            var cartItem = db.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }






        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            // Lấy iduser từ Session
            var iduser = Session["UserID"];
            if (iduser == null)
                return RedirectToAction("Login", "User");

            var cart = GetCart((int)iduser);
            ViewBag.TotalItemsInCart = GetTotalItemsInCart((int?)iduser);
            return View(cart);
        }


        // Lấy giỏ hàng của người dùng
        private Cart GetCart(int? iduser)
        {
            var cart = db.Carts
                .Include(c => c.CartItems)  // Đảm bảo rằng các CartItems được bao gồm
                .FirstOrDefault(c => c.CusID == iduser);

            if (cart == null)
            {
                cart = new Cart
                {
                    CusID = iduser,
                    CreatedDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    CartItems = new List<CartItem>()
                };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            return cart;
        }



        private int GetTotalItemsInCart(int? userId)
        {
            if (userId == null) return 0;

            // Lấy CartID dựa trên UserID
            var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.CusID == userId);
            if (cart == null) return 0;

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            int totalItems = cart.CartItems.Count();
            return totalItems;
        }



        private void ClearCart(int? iduser)
        {
            if (iduser == null) return;

            // Lấy giỏ hàng của người dùng
            var cart = db.Carts.Include("CartItems")
                               .FirstOrDefault(c => c.CusID == iduser);

            if (cart != null && cart.CartItems != null)
            {
                // Xóa từng mục trong giỏ hàng
                db.CartItems.RemoveRange(cart.CartItems);
                db.SaveChanges();
            }
        }





        [HttpGet]
        public ActionResult Checkout(int? iduser)
        {
            if (iduser == null)
            {
                return RedirectToAction("Login", "User");  // Chuyển hướng đến trang đăng nhập nếu người dùng chưa đăng nhập
            }

            // Lấy giỏ hàng của người dùng
            var cart = GetCart(iduser);

            // Kiểm tra nếu giỏ hàng trống
            if (cart == null || !cart.CartItems.Any())
            {
                return View("EmptyCart");  // Nếu giỏ hàng trống, trả về view thông báo giỏ hàng trống
            }

            // Lấy danh sách CartItems
            var cartItems = cart.CartItems.ToList();

            // Tính tổng tiền của giỏ hàng
            decimal total = (decimal)cartItems.Sum(item => item.Total);

            // Truyền danh sách CartItems và tổng tiền vào View
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = total;

            return View(cartItems);  // Truyền CartItems vào view
        }


        [HttpPost]
        public ActionResult Checkout(int? iduser, string paymentMethod, string valuecoupon, int? idcoupon)
        {
            if (iduser == null) return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(paymentMethod))
            {
                return Json(new { success = false });
            }

            var idcart = GetCart(iduser)?.CartID; // Lấy CartID từ đối tượng Cart
            var cartitems = db.CartItems.Where(c => c.CartID == idcart).ToList();

            if (cartitems.Count > 0)
            {
                // Duyệt qua danh sách trong giỏ hàng kiểm tra số lượng hợp lệ

                var flagquan = 0;
                foreach (var item in cartitems)
                {
                    // Tìm sản phẩm theo MenuItemID
                    var pro = db.MenuItems.FirstOrDefault(p => p.ItemID == item.MenuItemID);

                    // Kiểm tra nếu sản phẩm tồn tại (pro != null) và so sánh số lượng
                    if (pro != null && item.Quantity > pro.Quantity)
                    {
                        flagquan++;  // Tăng flag nếu số lượng trong giỏ hàng lớn hơn số lượng tồn kho
                    }
                }

                // Nếu có sản phẩm nào vượt quá số lượng tồn kho
                if (flagquan > 0)
                {
                    // Trả về thông báo lỗi
                    return Json(new { success = false, error = true, Flagquan = 1 });
                }

                else
                {
                    // Tính tổng tiền hàng và mã giảm giá 
                    decimal sumitems = 0;
                    decimal decreasevalue = 0;
                    foreach (var item in cartitems)
                    {
                        sumitems += (decimal)item.Total;
                    }

                    if (String.IsNullOrEmpty(valuecoupon))
                    {
                        decreasevalue = 0;
                    }
                    else
                    {
                        decreasevalue = int.Parse(valuecoupon);
                    }
                    sumitems = sumitems - ((decreasevalue * sumitems) / 100);

                    // Tạo mới đơn hàng 
                    var newoder = new Order
                    {
                        CusID = iduser,
                        CouponID = (idcoupon == null) ? null : idcoupon,
                        Total = sumitems,
                        Status = "0",
                        OrderDate = DateTime.Now,
                        DeliveryDate = null,
                        DeliveryAddress = null,
                        PaymentID = int.Parse(paymentMethod),
                    };
                    db.Orders.Add(newoder);
                    db.SaveChanges();

                    // Tạo mới chi tiết đơn 
                    foreach (var item in cartitems)
                    {
                        var neworderdetail = new OrderDetail
                        {
                            OrderID = newoder.OrderID,
                            MenuItemID = item.MenuItemID,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            Total = item.Total,
                        };
                        db.OrderDetails.Add(neworderdetail);

                        // Điều chỉnh lại số lượng sản phẩm sau khi đặt hàng
                        var existproduct = db.MenuItems.FirstOrDefault(p => p.ItemID == item.MenuItemID);
                        existproduct.Quantity -= item.Quantity;
                        db.Entry(existproduct).State = EntityState.Modified;
                    }
                    // Xóa giỏ hàng user 
                    ClearCart(iduser);
                    db.SaveChanges();

                    // Tạo mới hóa đơn
                    int valuepayment = int.Parse(paymentMethod);

                    //EWallet
                    if (valuepayment == 1)
                    {
                        var newinvoice = new Invoice
                        {
                            OrderID = newoder.OrderID,
                            Total = newoder.Total,
                            CreatedDate = DateTime.Now,
                            Status = "0",
                            PaymentID = 2,
                        };
                        newoder.Status = "1";
                        db.Entry(newoder).State = EntityState.Modified;
                        db.Invoices.Add(newinvoice);
                        db.SaveChanges();
                    }
                    // Direct method
                    else if (valuepayment == 2)
                    {
                        var newinvoice = new Invoice
                        {
                            OrderID = newoder.OrderID,
                            Total = newoder.Total,
                            CreatedDate = DateTime.Now,
                            Status = "1",
                            PaymentID = 1,
                        };
                        db.Invoices.Add(newinvoice);
                        db.SaveChanges();
                    }
                    // [CreditCard]
                    else if (valuepayment == 3)
                    {
                        var newinvoice = new Invoice
                        {
                            OrderID = newoder.OrderID,
                            Total = newoder.Total,
                            CreatedDate = DateTime.Now,
                            Status = "1",
                            PaymentID = 3,
                        };
                        db.Invoices.Add(newinvoice);
                        db.SaveChanges();
                    }
                    return Json(new { success = true });
                }
            }
            else
            {
                return Json(new { success = false });
            }
        }


    }
}
