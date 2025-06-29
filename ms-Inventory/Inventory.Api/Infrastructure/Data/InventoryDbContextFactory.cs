// Inventory.Api/Infrastructure/Data/InventoryDbContextFactory.cs
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
            // Carga appsettings.json para obtener la cadena de conexión
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var opts = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseNpgsql(cfg.GetConnectionString("Default"))
                // si quieres ignorar warnings de pending‐model aquí también:
                // .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                .Options;

            return new InventoryDbContext(opts);
        }
    }
}
