using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto);
        Task<ShoeDto?> GetShoeByIdAsync(int id);
        Task<IEnumerable<ShoeDto>> GetAllShoesAsync();
        Task UpdateShoeAsync(int id, CreateShoeDto dto);
        Task DeleteShoeAsync(int id);
        Task UpdateShoeStockAsync(int shoeId, int newStockQuantity);
        Task AddColorVariationAsync(int shoeId, int stockQuantity, string colorName, string? hexCode = null);
        Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync();
        Task<IEnumerable<InventoryReportDto>> GetLowStockAsync();
        Task<IEnumerable<object>> GetAllShoeVariationsAsync();
    }
}
