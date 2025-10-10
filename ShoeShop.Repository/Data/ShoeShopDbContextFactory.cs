using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;



namespace ShoeShop.Repository.Data
{
    public class ShoeShopDbContextFactory : IDesignTimeDbContextFactory<ShoeShopDbContext>
    {
        public ShoeShopDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ShoeShop.Web");
            var configFile = Path.Combine(basePath, "appsettings.json");

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");


            var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new ShoeShopDbContext(optionsBuilder.Options);
        }
    }
}
