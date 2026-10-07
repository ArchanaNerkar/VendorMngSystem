using VendorMngSystem.DbData;
using VendorMngSystem.Dtos;
using VendorMngSystem.IRepositorys;
using VendorMngSystem.Models;

namespace VendorMngSystem.Repositorys
{
    public class VendorDocRepository: IVendorDocRepository
    {
        private readonly DataDbContext _dataDbContext;
        public VendorDocRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }
        
        public void DeleteVendorsDcot(VendorDocuments vendorsDoc)
        {
            _dataDbContext.VendorDocuments.Remove(vendorsDoc);

        }
    }
}
