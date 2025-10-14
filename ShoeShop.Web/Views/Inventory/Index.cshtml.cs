// ShoeShop.Web/Pages/Inventory/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Web.Pages.Inventory
{
    public class IndexModel : PageModel
    {
        private readonly IInventoryService _inventoryService;

        public IndexModel(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public IEnumerable<InventoryReportDto> AllItems { get; set; } = new List<InventoryReportDto>();
        public IEnumerable<InventoryReportDto> LowStockItems { get; set; } = new List<InventoryReportDto>();

        public async Task OnGetAsync()
        {
            AllItems = await _inventoryService.GetInventoryReportAsync();
            LowStockItems = AllItems.Where(i => i.IsLowStock).ToList();
        }
    }
}