//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Restaurant_QKA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InventoryTransaction
    {
        public int ITID { get; set; }
        public int MaterialID { get; set; }
        public int StaffID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
    
        public virtual StaffWareHouse StaffWareHouse { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
