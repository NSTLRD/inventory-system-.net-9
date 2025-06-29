using System.Linq;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Repositories
{
    public class EfRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly InventoryDbContext _ctx;
        public EfRepository(InventoryDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);
        public void Update(T entity)     => _ctx.Set<T>().Update(entity);
        public void Remove(T entity)     => _ctx.Set<T>().Remove(entity);
        public IQueryable<T> Query()     => _ctx.Set<T>().AsNoTracking();
    }
}
