using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Api.Common.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
    }
}
