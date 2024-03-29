//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhotoStudio.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOrder
    {
        public int OrderID { get; set; }
        public long QuotationID { get; set; }
        public int PhotographerID { get; set; }
        public int PhotographerTypeID { get; set; }
        public Nullable<decimal> TotalPay { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDateTime { get; set; }
    
        public virtual tblPhotographer tblPhotographer { get; set; }
        public virtual tblPhotographerType tblPhotographerType { get; set; }
        public virtual tblQuotation tblQuotation { get; set; }
    }
}
