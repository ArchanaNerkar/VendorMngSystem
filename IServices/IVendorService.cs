using Microsoft.AspNetCore.Mvc.Rendering;
using VendorMngSystem.Dtos;
using VendorMngSystem.ViewModel;
using VendorMngSystem.ViewModels;

namespace VendorMngSystem.IServices
{
    public interface IVendorService
    {
        Task<List<VendorsDto>> GetAllVendors();
        Task<VendorsDto> GetVendorById(int id);
        Task<(int rowChanged, int vendorId)> CreateUpdateVendor(VendorsDto dto, string webRootPath);
        Task<List<SelectListItem>> GetVendorCategoriesDropDown();
        Task<int> DeleteVendorRecord(int id);
        Task<object> GetVendorsDataTable(DatatableParameters parameters);
        Task<List<VendorDocumentsDto>> VendorDocumentsList(int vendorId);

    }
}
