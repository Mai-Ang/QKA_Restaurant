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
    
    public partial class DiscountDetail
    {
        public int CoupontID { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> Amount { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Coupon Coupon { get; set; }
    }
}
