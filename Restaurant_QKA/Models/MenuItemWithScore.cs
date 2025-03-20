using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant_QKA.Models
{
    public class MenuItemWithScore
    {
        public MenuItem MenuItem { get; set; }
        public double Score { get; set; }
    }
}