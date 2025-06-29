using Microsoft.EntityFrameworkCore;
using Products.Api.Common.Interfaces;
using Products.Api.Infrastruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Products.Api.Infrastruture.Repositories
{
    public class EfRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ProductDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            // FindAsync es el método más eficiente para buscar por clave primaria
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void UpdateAsync(T entity)
        {
            // EF Core rastrea los cambios, solo necesitamos marcar la entidad como modificada
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void DeleteAsync(T entity)
        {
            // EF Core rastrea los cambios, solo necesitamos marcar la entidad para ser eliminada
            _dbSet.Remove(entity);
        }
    }
}
