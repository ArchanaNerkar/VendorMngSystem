namespace VendorMngSystem.IRepositorys
{
    public interface IUnitOfWork:IDisposable
    {
       
           IVendorsRepository _vendorsRepository { get; }
           IVendorDocRepository _vendorDocRepository { get; }
            Task BeginTransactionAsync();
            Task<int> CommitTransaction();
            Task RollBack();
            void Dispose();
        
    }
}

