//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Food.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PromoDetail
    {
        public int ProDetailID { get; set; }
        public int PromotionID { get; set; }
        public int ProductID { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
