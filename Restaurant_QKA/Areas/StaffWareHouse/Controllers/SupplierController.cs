using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class SupplierController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/Supplier
        public ActionResult Index()
        {
            List<Supplier> suppliers = db.Suppliers.ToList();
            return View(suppliers);
        }
    }
}