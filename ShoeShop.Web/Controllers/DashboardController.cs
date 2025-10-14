using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeShop.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await _dashboardService.GetDashboardDataAsync();

            var viewModel = new DashboardViewModel
            {
                TotalShoes = dto.TotalShoes,
                TotalColorVariations = dto.TotalColorVariations,
                LowStockItems = dto.LowStockItems,
                PendingPullOuts = dto.PendingPullOuts,
                TotalInventoryValue = dto.TotalInventoryValue,
                StockAlerts = dto.StockAlerts.Select(sa => new StockAlert
                {
                    ShoeName = sa.ShoeName,
                    ColorName = sa.ColorName,
                    CurrentStock = sa.CurrentStock,
                    ReorderLevel = sa.ReorderLevel
                }).ToList(),
                RecentActivities = dto.RecentActivities.Select(ra => new RecentActivity
                {
                    Action = ra.Action,
                    Description = ra.Description,
                    Timestamp = ra.Timestamp
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
