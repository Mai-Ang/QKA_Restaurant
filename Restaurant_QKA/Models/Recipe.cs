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
    
    public partial class Recipe
    {
        public int RecipeID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> CreatedStaff { get; set; }
        public Nullable<int> MaterialID { get; set; }
        public Nullable<double> Quantity { get; set; }
    
        public virtual MenuItem MenuItem { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
