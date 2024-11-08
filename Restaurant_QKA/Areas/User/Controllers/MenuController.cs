using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class MenuController : Controller
    {
        // GET: User/Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetailMenuItem()
        {
            return View();
        }
    }
}