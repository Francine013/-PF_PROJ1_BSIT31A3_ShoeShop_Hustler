using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeShop.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly ShoeShopDbContext _db;

        public ReportService(ShoeShopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InventoryReportDto>> GetInventorySummaryAsync()
        {
            var q = from cv in _db.ShoeColorVariations
                    join s in _db.Shoes on cv.ShoeId equals s.Id
                    select new InventoryReportDto
                    {
                        ShoeColorVariationId = cv.Id,
                        ShoeName = s.Name,
                        BrandName = s.Brand,
                        CategoryName = "N/A", // wala pang category table
                        ColorName = cv.ColorName,
                        StockQuantity = cv.StockQuantity,
                        ReorderLevel = cv.ReorderLevel,
                        SoldQuantity = _db.PurchaseOrderItems
                                        .Where(poi => poi.ShoeColorVariationId == cv.Id)
                                        .Sum(poi => (int?)poi.QuantityOrdered) ?? 0,
                        Revenue = _db.PurchaseOrderItems
                                        .Where(poi => poi.ShoeColorVariationId == cv.Id)
                                        .Sum(poi => (decimal?)(poi.QuantityOrdered * s.Price)) ?? 0m
                    };

            return await q.AsNoTracking().ToListAsync();
        }

        public async Task<decimal> GetInventoryValueAsync()
        {
            var q = from cv in _db.ShoeColorVariations
                    join s in _db.Shoes on cv.ShoeId equals s.Id
                    select (decimal)cv.StockQuantity * s.Cost;

            return await q.SumAsync();
        }
    }
}
