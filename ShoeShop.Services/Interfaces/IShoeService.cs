using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces.IShoeService
{
    public interface IShoeService
    {
        // Get all shoes (for listing in Inventory)
        Task<IEnumerable<ShoeDto>> GetAllShoesAsync();

        // Get a single shoe by its ID (for Edit, Details, or Delete confirmation)
        Task<ShoeDto?> GetShoeByIdAsync(int shoeId);

        // Create a new shoe (used in Create action)
        Task CreateShoeAsync(ShoeDto newShoe);

        // Update an existing shoe (used in Edit action)
        Task UpdateShoeAsync(ShoeDto updatedShoe);

        // Delete an existing shoe (used in Delete action)
        Task DeleteShoeAsync(int shoeId);

        // ✅ New: Update the stock of a shoe (e.g., after sale or restock)
        Task UpdateShoeStockAsync(int shoeId, int newStockQuantity);
    }
}
