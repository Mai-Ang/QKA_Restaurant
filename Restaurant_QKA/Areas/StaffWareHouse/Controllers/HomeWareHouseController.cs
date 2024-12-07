using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class HomeWareHouseController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/HomeWareHouse
        public ActionResult Index()
        {
            List<WareHouse> listwarehouse = db.WareHouses.ToList();
            ViewBag.Suppliers = db.Suppliers.ToList();
            return View(listwarehouse);
        }
    }
}