using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task ReceivePurchaseOrderAsync(int purchaseOrderId);
        Task<PurchaseOrderDto?> GetByIdAsync(int id);
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync();

        Task UpdatePurchaseOrderAsync(int id, PurchaseOrderDto dto);

        Task DeletePurchaseOrderAsync(int id);
    }
}