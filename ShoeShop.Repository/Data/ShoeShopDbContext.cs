using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Entities;
using ShoeShop.Repository.Entities.Enums;

namespace ShoeShop.Repository.Data
{
    public class ShoeShopDbContext : IdentityDbContext<IdentityUser>
    {
        public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options) : base(options)
        {
        }


        public DbSet<Shoe> Shoes { get; set; }
        public DbSet<ShoeColorVariation> ShoeColorVariations { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<StockPullOut> StockPullOuts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shoe>()
                .HasMany(s => s.ColorVariations)
                .WithOne(cv => cv.Shoe)
                .HasForeignKey(cv => cv.ShoeId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.PurchaseOrders)
                .WithOne(po => po.Supplier)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.OrderItems)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoeColorVariation>()
                .HasMany(scv => scv.PurchaseOrderItems)
                .WithOne(poi => poi.ShoeColorVariation)
                .HasForeignKey(poi => poi.ShoeColorVariationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShoeColorVariation>()
                .HasMany(scv => scv.StockPullOuts)
                .WithOne(spo => spo.ShoeColorVariation)
                .HasForeignKey(spo => spo.ShoeColorVariationId)
                .OnDelete(DeleteBehavior.Restrict);

            SeedStaticData(modelBuilder);
        }

        private void SeedStaticData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Nike Distribution Inc.", ContactEmail = "orders@nike.com", ContactPhone = "+1-800-NIKE", Address = "One Bowerman Drive, Beaverton, OR 97005", IsActive = true },
                new Supplier { Id = 2, Name = "Adidas Supply Chain", ContactEmail = "supply@adidas.com", ContactPhone = "+49-9132-84-0", Address = "Adi-Dassler-Strasse 1, Herzogenaurach, Germany", IsActive = true },
                new Supplier { Id = 3, Name = "Puma Wholesale Partners", ContactEmail = "wholesale@puma.com", ContactPhone = "+49-9132-81-0", Address = "PUMA Way 1, Herzogenaurach, Germany", IsActive = true }
            );
            DateTime created = new DateTime(2024, 1, 15);
            modelBuilder.Entity<Shoe>().HasData(
                new Shoe { Id = 1, Name = "Air Max 90", Brand = "Nike", Cost = 65.00m, Price = 120.00m, Description = "Classic Nike Air Max with iconic design", ImageUrl = "/images/shoes/airmax90.jpg", IsActive = true, CreatedDate = created },
                new Shoe { Id = 2, Name = "Air Force 1", Brand = "Nike", Cost = 70.00m, Price = 130.00m, Description = "Legendary basketball shoe", ImageUrl = "/images/shoes/airforce1.jpg", IsActive = true, CreatedDate = created }
            );

            modelBuilder.Entity<ShoeColorVariation>().HasData(
                new ShoeColorVariation { Id = 1, ShoeId = 1, ColorName = "White/Red", HexCode = "#FFFFFF", StockQuantity = 25, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 2, ShoeId = 2, ColorName = "Black/White", HexCode = "#000000", StockQuantity = 30, ReorderLevel = 5, IsActive = true }
            );

        }
    }
}
