using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;

namespace ShoeShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPullOutService _pullOutService;
        private readonly IReportService _reportService;

        public HomeController(
            IInventoryService inventoryService,
            IPullOutService pullOutService,
            IReportService reportService)
        {
            _inventoryService = inventoryService;
            _pullOutService = pullOutService;
            _reportService = reportService;
        }


        public async Task<IActionResult> Index()
        {
            try
            {

                var allShoes = await _inventoryService.GetAllShoesAsync();
                var inventoryReport = await _reportService.GetInventorySummaryAsync();
                var lowStock = await _inventoryService.GetLowStockAsync();
                var pendingPullOuts = await _pullOutService.GetPendingPullOutsAsync();
                var inventoryValue = await _reportService.GetInventoryValueAsync();

                var viewModel = new DashboardViewModel
                {
                    TotalShoes = allShoes.Count(),
                    TotalColorVariations = inventoryReport.Count(),
                    LowStockItems = lowStock.Count(),
                    PendingPullOuts = pendingPullOuts.Count(),
                    TotalInventoryValue = inventoryValue,

                    StockAlerts = lowStock.Take(5).Select(x => new StockAlert
                    {
                        ShoeName = x.ShoeName,
                        ColorName = x.ColorName,
                        CurrentStock = x.StockQuantity,
                        ReorderLevel = x.ReorderLevel
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while loading the dashboard: " + ex.Message;

                return View(new DashboardViewModel());
            }
        }
    }
}
