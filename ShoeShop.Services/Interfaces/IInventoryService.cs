using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces
{
    public interface IInventoryService
    {
        // --- Existing Methods ---
        Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto);
        Task<ShoeDto?> GetShoeByIdAsync(int id);
        Task<IEnumerable<ShoeDto>> GetAllShoesAsync();
        Task UpdateShoeAsync(int id, CreateShoeDto dto);
        Task DeleteShoeAsync(int id);

        Task UpdateShoeStockAsync(int shoeId, int newStockQuantity);

        Task AddColorVariationAsync(int shoeId, int stockQuantity, string colorName, string? hexCode = null);
        Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync();
        Task<IEnumerable<InventoryReportDto>> GetLowStockAsync();

        // ✅ FIX: Added the method required by the PurchaseOrderController
        Task<IEnumerable<object>> GetAllShoeVariationsAsync();
    }
}