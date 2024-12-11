using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffChef.Controllers
{
    public class WareHouseController : Controller
    {
        // GET: StaffChef/WareHouse
        Restaurant_Entities db = new Restaurant_Entities();
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "User", new { area = "User" });
            // Lấy dữ liệu từ bảng WareHouse
            var wareHouseList = db.WareHouses.ToList();
            return View(wareHouseList);
        }
    }
}