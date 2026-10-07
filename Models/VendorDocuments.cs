using System.ComponentModel.DataAnnotations;

namespace VendorMngSystem.Models
{
    public class VendorDocuments
    {
        [Key]
        public int VendorsDocumentId { get;set; }
        public string VendorFileName { get;set; }
        public string VendorFilePath { get;set; }
        public int VendorId { get; set; }

        public Vendors? vendors { get; set; }

    }
}
