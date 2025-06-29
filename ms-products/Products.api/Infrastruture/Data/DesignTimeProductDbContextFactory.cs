using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Products.Api.Infrastruture.Data
{
    public class DesignTimeProductDbContextFactory
        : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            // Leemos la misma appsettings.json que en tiempo de ejecución
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ProductDbContext>();
            var connStr = config.GetConnectionString("Default");
            builder.UseNpgsql(connStr, o => 
                o.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName));

            return new ProductDbContext(builder.Options);
        }
    }
}
