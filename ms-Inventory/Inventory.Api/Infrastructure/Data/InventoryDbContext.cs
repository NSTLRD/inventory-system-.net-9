using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Repositories;

namespace Inventory.Api.Infrastructure.Data
{
    public class InventoryDbContext : DbContext, IUnitOfWork
    {
        
        private IDbContextTransaction? _currentTransaction;

        public DbSet<InventoryItem>     InventoryItems     { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> opts)
            : base(opts)
        { }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return new EfRepository<T>(this);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.ProductId)
                .IsUnique();

            modelBuilder.Entity<InventoryMovement>()
                .HasOne<InventoryItem>()
                .WithMany(i => i.Movements)
                .HasForeignKey(m => m.InventoryItemId);
                
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default) 
            => base.SaveChangesAsync(ct);
    }
}
