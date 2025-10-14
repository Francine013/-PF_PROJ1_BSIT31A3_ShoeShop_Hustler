// ShoeShop.Web/Pages/PurchaseOrders/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Web.Pages.PurchaseOrders
{
    public class IndexModel : PageModel
    {
        private readonly IPurchaseOrderService _poService;

        public IndexModel(IPurchaseOrderService poService)
        {
            _poService = poService;
        }

        public IEnumerable<PurchaseOrderDto> PurchaseOrders { get; set; } = new List<PurchaseOrderDto>();

        public async Task OnGetAsync()
        {
            PurchaseOrders = await _poService.GetAllAsync();
        }
    }
}