using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse.Controllers
{
    public class TransactionController : Controller
    {
        Restaurant_Entities db = new Restaurant_Entities();
        // GET: StaffWareHouse/Transaction
        public ActionResult Index()
        {
            List<InventoryTransaction> transactions = db.InventoryTransactions.ToList();
            return View(transactions);
        }
    }
}