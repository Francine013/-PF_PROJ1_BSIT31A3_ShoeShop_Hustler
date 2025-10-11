using ShoeShop.Services.DTOs;

namespace ShoeShop.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<InventoryReportDto>> GetInventorySummaryAsync();
        Task<decimal> GetInventoryValueAsync();
    }
}
