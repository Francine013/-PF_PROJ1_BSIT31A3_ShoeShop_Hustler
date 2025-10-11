using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces
{
    // The interface the PurchaseOrderController requires for supplier lookup
    // ✅ FIXED: Pinalitan ang return type mula sa 'object' patungong 'SupplierDto'
    public interface ISupplierService
    {
        // Ang tamang signature ay dapat tumanggap ng SupplierDto, hindi generic object.
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
    }
}