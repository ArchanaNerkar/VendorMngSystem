using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VendorMngSystem.Models;
using VendorMngSystem.ViewModels;

namespace VendorMngSystem.Dtos
{
    public class VendorsDto
    {
            public int VendorId { get; set; }
        [Required(ErrorMessage = "Vendor Name is mandatory.")]
        [StringLength(100, ErrorMessage = "Vendor name must be less than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.]+$", ErrorMessage = "Vendor name can only contain letters, dots, and spaces.")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
        [Required(ErrorMessage = "Contact Person is mandatory.")]
        [StringLength(100, ErrorMessage = "Contact Person must be less than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.]+$", ErrorMessage = "Vendor name can only contain letters, dots, and spaces.")]
        [Display(Name = "Contact Person")] 
        public string ContactPerson { get; set; }
        
        [Required(ErrorMessage = "Email Address is mandatory.")]
        [StringLength(150, ErrorMessage = "Email must be less than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")] 
        public string Emails { get; set; }
        [Required(ErrorMessage = "Mobile Number is mandatory.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit numeric value.")]
        [Display(Name = "Mobile Number")] 
        public string MobileNumbers { get; set; }
        [Required(ErrorMessage = "GST Number is mandatory.")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
        ErrorMessage = "valid 15-character")]//GSTIN (e.g., 22AAAAA0000A1Z5).
        [Display(Name = "GST Number")]
        public string GSTNumber { get; set; }
        [Required(ErrorMessage = "Please select a Vendor Category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Vendor Category.")]
        [Display(Name = "Vendor Category")]
        public int VendorsCategoryId { get; set; }

        [Required(ErrorMessage = "Address is mandatory.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string VendorAddress { get; set; }
        //[Required(ErrorMessage = "Select Status")]
        //[Display(Name = "Status")]
        public bool VendorStatus { get; set; } = true;

        //All nevigation 
        [ValidateNever]
        public VendorsCategory? vendorsCategories { get; set; } //= new VendorsCategory();

       //List display
        [ValidateNever]
        public List<SelectListItem> vendorCategoriesList { get; set; } = new List<SelectListItem>();
        public List<IFormFile>? VendorDocumentsPath { get; set; }

        // Existing files displayed to the user
        //public List<VendorDocumentsViewModel> ExistingDocuments { get; set; } = new();
        [ValidateNever]
        public List<VendorDocumentsDto> ExistingDocuments { get; set; } = new();

        // The list of IDs the user STILL KEPT in the UI (posted back)
        public List<int> KeptDocumentIds { get; set; } = new();
    }
}
