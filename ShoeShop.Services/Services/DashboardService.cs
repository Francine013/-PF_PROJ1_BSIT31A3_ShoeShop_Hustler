using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeShop.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ShoeShopDbContext _db;

        public DashboardService(ShoeShopDbContext db) => _db = db;

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var dashboard = new DashboardDto
            {
                TotalShoes = await _db.Shoes.CountAsync(),
                TotalColorVariations = await _db.ShoeColorVariations.CountAsync(),
                LowStockItems = await _db.ShoeColorVariations
                                      .CountAsync(cv => cv.StockQuantity <= cv.ReorderLevel),
                PendingPullOuts = await _db.StockPullOuts
                                      .CountAsync(spo => spo.Status == Repository.Entities.Enums.PullOutStatus.Pending),
                TotalInventoryValue = await _db.ShoeColorVariations
                                          .Join(_db.Shoes,
                                                cv => cv.ShoeId,
                                                s => s.Id,
                                                (cv, s) => new { cv.StockQuantity, s.Cost })
                                          .SumAsync(x => x.StockQuantity * x.Cost)
            };

            // Stock Alerts
            dashboard.StockAlerts = await _db.ShoeColorVariations
                .Where(cv => cv.StockQuantity <= cv.ReorderLevel)
                .Join(_db.Shoes,
                      cv => cv.ShoeId,
                      s => s.Id,
                      (cv, s) => new StockAlertDto
                      {
                          ShoeName = s.Name,
                          ColorName = cv.ColorName,
                          CurrentStock = cv.StockQuantity,
                          ReorderLevel = cv.ReorderLevel
                      })
                .ToListAsync();

            // Recent Activities (last 5 pull-outs)
            dashboard.RecentActivities = await _db.StockPullOuts
                .OrderByDescending(spo => spo.RequestedDate)
                .Take(5)
                .Select(spo => new RecentActivityDto
                {
                    Action = spo.Status.ToString(),
                    Description = $"Pull-out request for ShoeColorVariation ID {spo.ShoeColorVariationId}",
                    Timestamp = spo.RequestedDate
                })
                .ToListAsync();

            return dashboard;
        }
    }
}
