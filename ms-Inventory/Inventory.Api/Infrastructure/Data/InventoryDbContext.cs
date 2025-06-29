using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Repositories;

namespace Inventory.Api.Infrastructure.Data
{
    public class InventoryDbContext : DbContext, IUnitOfWork
    {
        public DbSet<InventoryItem>     InventoryItems     { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> opts)
            : base(opts)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // InventoryItem
            modelBuilder.Entity<InventoryItem>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<InventoryItem>()
                .Property(x => x.ProductId).IsRequired();
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.Stock) // En lugar de Quantity
                .IsRequired();
            modelBuilder.Entity<InventoryItem>()
                .HasMany(x => x.Movements)
                .WithOne()
                .HasForeignKey(m => m.InventoryItemId);

            // InventoryMovement
            modelBuilder.Entity<InventoryMovement>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<InventoryMovement>()
                .Property(x => x.QuantityChange).HasColumnName("QuantityChange");
            modelBuilder.Entity<InventoryMovement>()
                .Property(x => x.Timestamp).HasColumnName("Timestamp");

            base.OnModelCreating(modelBuilder);
        }

        // Aseguramos el override para no esconder el método de DbContext
        public override Task<int> SaveChangesAsync(CancellationToken ct = default) 
            => base.SaveChangesAsync(ct);

        // Devuelve tu repositorio genérico
        public IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class
            => new EfRepository<TEntity>(this);
    }
}
