using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VendorMngSystem.DbData;
using VendorMngSystem.IRepositorys;

namespace VendorMngSystem.Repositorys
{
    public class UnitOfWork:IUnitOfWork
    {
        public IVendorsRepository _vendorsRepository { get; }
        public IVendorDocRepository _vendorDocRepository { get; }
        private readonly DataDbContext _dataDbContext;
        private IDbContextTransaction _dbContextTransaction;
        public UnitOfWork(DataDbContext dataDbContext, IVendorsRepository vendorsRepository,
            IVendorDocRepository vendorDocRepository)
        {

            _dataDbContext = dataDbContext;
            _vendorsRepository = vendorsRepository;
            _vendorDocRepository = vendorDocRepository;
        }
        public async Task BeginTransactionAsync()
        {
            _dbContextTransaction = await _dataDbContext.Database.BeginTransactionAsync();

        }

        public async Task<int> CommitTransaction()
        {
            try
            {
                int rowChangewd = await _dataDbContext.SaveChangesAsync();
                if (_dbContextTransaction != null)
                {
                    await _dbContextTransaction.CommitAsync();
                    await _dbContextTransaction.DisposeAsync();

                }
                return rowChangewd;
            }
            catch (Exception e)
            {
                await _dbContextTransaction.RollbackAsync();
                await _dbContextTransaction.DisposeAsync();
                throw;
            }

        }
        public async Task RollBack()
        {
            await _dbContextTransaction.RollbackAsync();
            await _dbContextTransaction.DisposeAsync();


        }

        public void Dispose()
        {

            _dbContextTransaction?.Dispose();
        }

    }
}
