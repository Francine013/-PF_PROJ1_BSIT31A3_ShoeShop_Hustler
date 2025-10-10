using Microsoft.AspNetCore.Mvc;

using ShoeShop.Services.Interfaces;

using ShoeShop.Web.Models.ViewModels;

using System.Linq;

using System.Threading.Tasks;



namespace ShoeShop.Web.Controllers

{

    public class ReportController : Controller

    {

        private readonly IReportService _reportService;

        private readonly IInventoryService _inventoryService;



        public ReportController(IReportService reportService, IInventoryService inventoryService)

        {

            _reportService = reportService;

            _inventoryService = inventoryService;

        }



        public async Task<IActionResult> Index()

        {

            var reportData = await _reportService.GetInventorySummaryAsync();



            var viewModel = reportData.Select(r => new ReportViewModel

            {

                ProductName = r.ShoeName,            

                BrandName = r.BrandName ?? "",       

                CategoryName = r.CategoryName ?? "",

                StockQuantity = r.StockQuantity,

                SoldQuantity = r.SoldQuantity,

                Revenue = r.Revenue

            }).ToList();



            return View(viewModel);

        }



        public async Task<IActionResult> LowStock()

        {

            var lowStockData = await _inventoryService.GetLowStockAsync();



            var viewModel = lowStockData.Select(r => new LowStockReportViewModel

            {

                ProductName = r.ShoeName,

                BrandName = r.BrandName ?? "",

                StockQuantity = r.StockQuantity,

                ReorderLevel = r.ReorderLevel

            }).ToList();



            return View(viewModel);

        }

    }

}