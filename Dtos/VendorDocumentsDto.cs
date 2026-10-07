using System.ComponentModel.DataAnnotations;

namespace VendorMngSystem.Dtos
{
    public class VendorDocumentsDto
    {
        [Key]
        public int VendorsDocumentId { get;set; }
        public string VendorFileName { get;set; }
        public string VendorFilePath { get;set; }
        public int VendorId { get; set; }

        public VendorsDto? vendorsdto { get; set; }

    }
}
