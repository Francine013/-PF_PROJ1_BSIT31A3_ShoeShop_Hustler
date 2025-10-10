using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ShoeShop.Repository.Data;

namespace ShoeShop.Repository.Console
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShoeShopDbContext>
    {
        public ShoeShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();

            // --- SQLite Configuration with Migrations Assembly ---
            // The MigrationsAssembly setting directs EF Core to put the migration files 
            // in this console project, even though the DbContext is in the Repository project.
            optionsBuilder.UseSqlite("Data Source=ShoeShop.db",
                b => b.MigrationsAssembly("ShoeShop.Repository.Console"));

            return new ShoeShopDbContext(optionsBuilder.Options);
        }
    }
}