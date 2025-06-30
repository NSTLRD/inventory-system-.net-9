
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;
using Products.Api.Domain.ValueObjects;
using Products.Api.Infrastruture.Repositories;

namespace Products.Api.Infrastruture.Data
{
    public class ProductDbContext
        : IdentityDbContext<IdentityUser, IdentityRole, string>,
          IUnitOfWork
    {
        public DbSet<Product> Products { get; set; } = null!;

        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class
            => new EfRepository<TEntity>(this);

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).IsRequired();
                b.Property(p => p.Description).IsRequired();
                b.Property(p => p.Category).IsRequired();
                b.Property(p => p.Price)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();
                b.Property(p => p.SKU).IsRequired();

                var historyConverter = new ValueConverter<PriceHistory, string>(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<PriceHistory>(
                              v, new JsonSerializerOptions())!
                );

                b.Property(p => p.History)
                 .HasConversion(historyConverter)
                 .HasColumnType("jsonb")
                 .IsRequired();
            });
        }
    }
}
