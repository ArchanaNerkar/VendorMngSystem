using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VendorMngSystem.DbData;
using VendorMngSystem.Dtos;
using VendorMngSystem.IRepositorys;
using VendorMngSystem.Models;
using VendorMngSystem.ViewModels;

namespace VendorMngSystem.Repositorys
{
    public class VendorsRepository:IVendorsRepository
    {
        private readonly DataDbContext _dataDbContext;
        public VendorsRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }
        public  IQueryable<Vendors> GetVendors()
        {
            return  _dataDbContext.Vendors.Include(c => c.vendorsCategories).Include(x=>x.vendorDocuments).AsNoTracking();
        }
        public async Task<Vendors?> FindVendorsById(int id)
        {
            return await _dataDbContext.Vendors.FindAsync(id);
        }
        public async Task<Vendors?> FindVendorsWithVendorDocument(int id)
        {
            return await _dataDbContext.Vendors.Include(x=>x.vendorDocuments).FirstOrDefaultAsync(x=>x.VendorId==id);
           // return await _dataDbContext.Vendors.Include(x => x.vendorsCategories).Include(x => x.vendorDocuments).FirstOrDefaultAsync(x=>x.VendorId==id);
        }
        public async Task CreateVendor(Vendors vendors)
        { 
             await _dataDbContext.Vendors.AddAsync(vendors);
        
        }
        //public void  UpdateVendor(Vendors vendors)
        //{
        //     _dataDbContext.Vendors.Update(vendors);

        //}
        public void UpdateVendor(Vendors vendor)
        {
              if (_dataDbContext.Entry(vendor).State == EntityState.Detached)
            {
                _dataDbContext.Vendors.Attach(vendor);
                _dataDbContext.Entry(vendor).State = EntityState.Modified;
            }
        }
        public void DeleteVendorsDeatails(Vendors vendors)
        {
            _dataDbContext.Vendors.Remove(vendors);

        }

        //Other Entity Repo code
        public async Task<List<VendorsCategory>> VendorCategoriesDropDown()
        {
            return await _dataDbContext.VendorsCategories.AsNoTracking().ToListAsync();
        }

        // Reload existing documents if validation fails
        public async Task<List<VendorDocumentsDto>> VendorDocumentsList(int vendorId)
        {
            return await _dataDbContext.VendorDocuments
            .Where(d => d.VendorId == vendorId)
            .Select(d => new VendorDocumentsDto
            {
                VendorsDocumentId = d.VendorsDocumentId,
                VendorFileName = d.VendorFileName,
                VendorFilePath = d.VendorFilePath
            }).ToListAsync();
        }
        public void DeleteVendorsDcot(VendorDocuments vendorsDoc)
        {
            _dataDbContext.VendorDocuments.Remove(vendorsDoc);

        }
    }
}
