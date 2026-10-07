using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace VendorMngSystem.Models
{
    public class Vendors
    {
        [Key]
        public int VendorId {  get; set; }
        public string VendorName { get; set; }
        public string ContactPerson { get; set; }
        public string Emails { get; set; }
        public string MobileNumbers { get; set; }
        public string GSTNumber { get; set; }
        public int VendorsCategoryId { get; set; }

        public string VendorAddress { get; set; }
        public bool VendorStatus { get; set; } = true;

        //All nevigation 
        public VendorsCategory? vendorsCategories { get; set; }

        public ICollection<VendorDocuments> vendorDocuments = new List<VendorDocuments>();

    }
}
