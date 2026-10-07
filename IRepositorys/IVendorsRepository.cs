using Microsoft.EntityFrameworkCore.Migrations.Operations;
using VendorMngSystem.Dtos;
using VendorMngSystem.Models;
using VendorMngSystem.ViewModels;

namespace VendorMngSystem.IRepositorys
{
    public interface IVendorsRepository
    {

        public IQueryable<Vendors> GetVendors();
        public Task<Vendors?> FindVendorsById(int id);
        public Task<Vendors?> FindVendorsWithVendorDocument(int id);

        public Task CreateVendor(Vendors vendors);
        public void UpdateVendor(Vendors vendors);
        public void DeleteVendorsDeatails(Vendors vendors);
        // VendorCategory 
        Task<List<VendorsCategory>> VendorCategoriesDropDown();

        public Task<List<VendorDocumentsDto>> VendorDocumentsList(int vendorId);
       

    }
}
