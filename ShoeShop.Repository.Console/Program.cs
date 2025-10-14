using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Repository.Entities;
using ShoeShop.Repository.Entities.Enums;

namespace ShoeShop.Repository.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("🗄️ ShoeShop Repository Layer - Console Testing Application");
            System.Console.WriteLine("===========================================================\n");

            // Setup DbContext with SQLite
            var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();
            optionsBuilder.UseSqlite("Data Source=shoeshop.db");

            using var context = new ShoeShopDbContext(optionsBuilder.Options);

            System.Console.WriteLine("📦 Creating database and applying migrations...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            System.Console.WriteLine("✅ Database created successfully!\n");

            await TestShoeOperations(context);
            await TestColorVariationOperations(context);
            await TestSupplierOperations(context);
            await TestPurchaseOrderOperations(context);
            await TestStockPullOutOperations(context);
            await TestComplexQueries(context);
            await TestRelationships(context);

            System.Console.WriteLine("\n✅ All tests completed successfully!");
            System.Console.WriteLine("Database file: shoeshop.db");
        }

        static async Task TestShoeOperations(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 1: Shoe CRUD Operations");
            System.Console.WriteLine("================================");

   
            var newShoe = new Shoe
            {
                Name = "Test Shoe",
                Brand = "Test Brand",
                Cost = 50.00m,
                Price = 100.00m,
                Description = "Test shoe for console testing",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            context.Shoes.Add(newShoe);
            await context.SaveChangesAsync();
            System.Console.WriteLine($"✅ Created shoe: {newShoe.Name} (ID: {newShoe.Id})");
            
            var shoe = await context.Shoes.FindAsync(newShoe.Id);
            System.Console.WriteLine($"✅ Read shoe: {shoe?.Name}");

     
            if (shoe != null)
            {
                shoe.Price = 110.00m;
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Updated shoe price to: ${shoe.Price}");
            }


            if (shoe != null)
            {
                context.Shoes.Remove(shoe);
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Deleted shoe: {shoe.Name}\n");
            }
        }

        static async Task TestColorVariationOperations(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 2: Color Variation CRUD Operations");
            System.Console.WriteLine("==========================================");

            var shoe = await context.Shoes.FirstOrDefaultAsync();
            if (shoe != null)
            {
                var colorVariation = new ShoeColorVariation
                {
                    ShoeId = shoe.Id,
                    ColorName = "Test Color",
                    HexCode = "#FF0000",
                    StockQuantity = 100,
                    ReorderLevel = 10,
                    IsActive = true
                };

                context.ShoeColorVariations.Add(colorVariation);
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Created color variation: {colorVariation.ColorName} for {shoe.Name}");


                colorVariation.StockQuantity = 150;
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Updated stock quantity to: {colorVariation.StockQuantity}");

                context.ShoeColorVariations.Remove(colorVariation);
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Deleted color variation\n");
            }
        }

        static async Task TestSupplierOperations(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 3: Supplier CRUD Operations");
            System.Console.WriteLine("===================================");

            var supplier = new Supplier
            {
                Name = "Test Supplier Co.",
                ContactEmail = "test@supplier.com",
                ContactPhone = "+1-555-1234",
                Address = "123 Test Street, Test City",
                IsActive = true
            };

            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
            System.Console.WriteLine($"✅ Created supplier: {supplier.Name}");

            // Read and Update
            var foundSupplier = await context.Suppliers.FindAsync(supplier.Id);
            if (foundSupplier != null)
            {
                foundSupplier.ContactPhone = "+1-555-9999";
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Updated supplier phone: {foundSupplier.ContactPhone}\n");
            }
        }

        static async Task TestPurchaseOrderOperations(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 4: Purchase Order CRUD Operations");
            System.Console.WriteLine("=========================================");

            var supplier = await context.Suppliers.FirstOrDefaultAsync();
            var colorVariation = await context.ShoeColorVariations.FirstOrDefaultAsync();

            if (supplier != null && colorVariation != null)
            {
                var purchaseOrder = new PurchaseOrder
                {
                    OrderNumber = "TEST-PO-001",
                    SupplierId = supplier.Id,
                    OrderDate = DateTime.Now,
                    ExpectedDate = DateTime.Now.AddDays(14),
                    Status = OrderStatus.Pending,
                    TotalAmount = 0
                };

                context.PurchaseOrders.Add(purchaseOrder);
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Created purchase order: {purchaseOrder.OrderNumber}");

                // Add order items
                var orderItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = purchaseOrder.Id,
                    ShoeColorVariationId = colorVariation.Id,
                    QuantityOrdered = 50,
                    QuantityReceived = 0,
                    UnitCost = 75.00m
                };

                context.PurchaseOrderItems.Add(orderItem);
                purchaseOrder.TotalAmount = orderItem.QuantityOrdered * orderItem.UnitCost;
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Added order item: {orderItem.QuantityOrdered} units @ ${orderItem.UnitCost}");

                purchaseOrder.Status = OrderStatus.Confirmed;
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Updated order status to: {purchaseOrder.Status}\n");
            }
        }

        static async Task TestStockPullOutOperations(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 5: Stock Pull-Out CRUD Operations");
            System.Console.WriteLine("=========================================");

            var colorVariation = await context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .FirstOrDefaultAsync(cv => cv.StockQuantity > 10);

            if (colorVariation != null)
            {
                var pullOut = new StockPullOut
                {
                    ShoeColorVariationId = colorVariation.Id,
                    Quantity = 5,
                    Reason = "Testing",
                    ReasonDetails = "Console test pull-out",
                    RequestedBy = "Test User",
                    PullOutDate = DateTime.Now,
                    Status = PullOutStatus.Pending
                };

                context.StockPullOuts.Add(pullOut);
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Created pull-out request: {pullOut.Quantity} units of {colorVariation.Shoe.Name}");

                pullOut.Status = PullOutStatus.Approved;
                pullOut.ApprovedBy = "Manager Test";
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Approved pull-out by: {pullOut.ApprovedBy}");

                pullOut.Status = PullOutStatus.Completed;
                colorVariation.StockQuantity -= pullOut.Quantity;
                await context.SaveChangesAsync();
                System.Console.WriteLine($"✅ Completed pull-out, new stock: {colorVariation.StockQuantity}\n");
            }
        }

        static async Task TestComplexQueries(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 6: Complex Queries");
            System.Console.WriteLine("==========================");

            var lowStockItems = await context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.StockQuantity <= cv.ReorderLevel)
                .ToListAsync();

            System.Console.WriteLine($"📊 Low Stock Items: {lowStockItems.Count}");
            foreach (var item in lowStockItems.Take(5))
            {
                System.Console.WriteLine($"   - {item.Shoe.Name} ({item.ColorName}): {item.StockQuantity} units");
            }

            var pendingOrders = await context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.Status == OrderStatus.Pending || po.Status == OrderStatus.Confirmed)
                .ToListAsync();

            System.Console.WriteLine($"\n📦 Pending Orders: {pendingOrders.Count}");
            foreach (var order in pendingOrders.Take(5))
            {
                System.Console.WriteLine($"   - {order.OrderNumber} from {order.Supplier.Name}: ${order.TotalAmount}");
            }

            var totalInventoryValue = await context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .SumAsync(cv => cv.StockQuantity * cv.Shoe.Cost);

            System.Console.WriteLine($"\n💰 Total Inventory Value: ${totalInventoryValue:N2}");

            var shoesByBrand = await context.Shoes
                .GroupBy(s => s.Brand)
                .Select(g => new { Brand = g.Key, Count = g.Count() })
                .ToListAsync();

            System.Console.WriteLine($"\n🏷️ Shoes by Brand:");
            foreach (var group in shoesByBrand)
            {
                System.Console.WriteLine($"   - {group.Brand}: {group.Count} models");
            }
            System.Console.WriteLine();
        }

        static async Task TestRelationships(ShoeShopDbContext context)
        {
            System.Console.WriteLine("🔹 TEST 7: Relationship Navigation");
            System.Console.WriteLine("===================================");

            var shoeWithColors = await context.Shoes
                .Include(s => s.ColorVariations)
                .FirstOrDefaultAsync();

            if (shoeWithColors != null)
            {
                System.Console.WriteLine($"👟 {shoeWithColors.Name}");
                System.Console.WriteLine($"   Color variations: {shoeWithColors.ColorVariations.Count}");
                foreach (var color in shoeWithColors.ColorVariations.Take(3))
                {
                    System.Console.WriteLine($"   - {color.ColorName}: {color.StockQuantity} in stock");
                }
            }

            var orderWithDetails = await context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.ShoeColorVariation)
                        .ThenInclude(cv => cv.Shoe)
                .FirstOrDefaultAsync();

            if (orderWithDetails != null)
            {
                System.Console.WriteLine($"\n📦 Order: {orderWithDetails.OrderNumber}");
                System.Console.WriteLine($"   Supplier: {orderWithDetails.Supplier.Name}");
                System.Console.WriteLine($"   Items: {orderWithDetails.OrderItems.Count}");
                foreach (var item in orderWithDetails.OrderItems.Take(3))
                {
                    System.Console.WriteLine($"   - {item.ShoeColorVariation.Shoe.Name} ({item.ShoeColorVariation.ColorName}): {item.QuantityOrdered} units");
                }
            }

            var pullOutWithDetails = await context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                    .ThenInclude(cv => cv.Shoe)
                .FirstOrDefaultAsync();

            if (pullOutWithDetails != null)
            {
                System.Console.WriteLine($"\n📤 Pull-Out Request");
                System.Console.WriteLine($"   Shoe: {pullOutWithDetails.ShoeColorVariation.Shoe.Name}");
                System.Console.WriteLine($"   Color: {pullOutWithDetails.ShoeColorVariation.ColorName}");
                System.Console.WriteLine($"   Quantity: {pullOutWithDetails.Quantity}");
                System.Console.WriteLine($"   Reason: {pullOutWithDetails.Reason}");
                System.Console.WriteLine($"   Status: {pullOutWithDetails.Status}");
            }
        }
    }
}
