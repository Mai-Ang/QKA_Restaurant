using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: User/Home
        public ActionResult Index()
        {
            var menuItems = db.MenuItems.Where(m => m.IsActive ?? false).ToList();
            return View(menuItems); 
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}