
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Inventory.Api.Infrastructure.Data
{
    public class InventoryDbContextFactory
        : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var opts = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseNpgsql(cfg.GetConnectionString("Default"))
                .Options;

            return new InventoryDbContext(opts);
        }
    }
}
