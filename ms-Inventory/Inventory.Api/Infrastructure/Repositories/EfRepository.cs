using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Repositories
{
    public class EfRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly InventoryDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(InventoryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found");
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
