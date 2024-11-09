using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class CartsController : Controller
    {
        RestaurantEntities db = new RestaurantEntities();
        // GET: User/Carts
        public ActionResult Index(int? cusId)
        {
            // Retrieve customer ID from the session or parameters if necessary
            int customerId = cusId ?? 0; // Replace with actual customer ID retrieval method

            // Fetch items in the cart for the specified customer
            var cartItems = db.Carts
                              .Where(c => c.CusID == customerId)
                              .Select(c => new
                              {
                                  c.CartID,
                                  c.ItemID,
                                  c.Quantity,
                                  c.CusID
                              })
                              .ToList();

            return View(cartItems); // Pass data to the view
        }
    }
}