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
    
    public partial class tblPortfolioGallery
    {
        public int PortfolioGalleryID { get; set; }
        public int PortfolioID { get; set; }
        public int PortfolioGalleryCategoryID { get; set; }
        public string PortfolioGalleryImage { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual tblPortfolio tblPortfolio { get; set; }
        public virtual tblPortfolioGalleryCategory tblPortfolioGalleryCategory { get; set; }
    }
}
