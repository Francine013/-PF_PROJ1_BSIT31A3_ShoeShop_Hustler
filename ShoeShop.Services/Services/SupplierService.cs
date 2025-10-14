using ShoeShop.Services.Interfaces;
using ShoeShop.Services.DTOs; 
namespace ShoeShop.Services.Services
{
  
    public class SupplierService : ISupplierService
    {
        
        public Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            
            var suppliers = new List<SupplierDto>
            {
                new SupplierDto { Id = 1, Name = "Supplier A (Mock)" },
                new SupplierDto { Id = 2, Name = "Supplier B (Mock)" },
                new SupplierDto { Id = 3, Name = "Supplier C (Mock)" }
            };
            return Task.FromResult(suppliers.AsEnumerable());
        }

    }
}
