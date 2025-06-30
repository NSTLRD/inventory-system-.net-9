using System;
using System.Threading.Tasks;

namespace Inventory.Api.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
