using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeShop.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ShoeShopDbContext _db;

        public InventoryService(ShoeShopDbContext db) => _db = db;

        public async Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto)
        {
            var entity = new Shoe
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Cost = dto.Cost,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.UtcNow
            };
            _db.Shoes.Add(entity);
            await _db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task DeleteShoeAsync(int id)
        {
            var shoe = await _db.Shoes.FindAsync(id) ?? throw new KeyNotFoundException("Shoe not found");
            _db.Shoes.Remove(shoe);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoeDto>> GetAllShoesAsync()
        {
            return await _db.Shoes
                .AsNoTracking()
                .Select(s => ToDto(s))
                .ToListAsync();
        }

        public async Task<ShoeDto?> GetShoeByIdAsync(int id)
        {
            var s = await _db.Shoes.FindAsync(id);
            return s is null ? null : ToDto(s);
        }

        public async Task UpdateShoeAsync(int id, CreateShoeDto dto)
        {
            var entity = await _db.Shoes.FindAsync(id) ?? throw new KeyNotFoundException("Shoe not found");
            entity.Name = dto.Name;
            entity.Brand = dto.Brand;
            entity.Cost = dto.Cost;
            entity.Price = dto.Price;
            entity.Description = dto.Description;
            entity.ImageUrl = dto.ImageUrl;
            entity.IsActive = dto.IsActive;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateShoeStockAsync(int shoeId, int newStockQuantity)
        {
            var variation = await _db.ShoeColorVariations
                .FirstOrDefaultAsync(cv => cv.ShoeId == shoeId);

            if (variation == null)
                throw new KeyNotFoundException("Shoe color variation not found");

            variation.StockQuantity = newStockQuantity;
            await _db.SaveChangesAsync();
        }

        public async Task AddColorVariationAsync(int shoeId, int stockQuantity, string colorName, string? hexCode = null)
        {
            var shoe = await _db.Shoes.FindAsync(shoeId) ?? throw new KeyNotFoundException("Shoe not found");
            var cv = new ShoeColorVariation
            {
                ShoeId = shoeId,
                ColorName = colorName,
                HexCode = hexCode,
                StockQuantity = stockQuantity,
                ReorderLevel = 5,
                IsActive = true
            };
            _db.ShoeColorVariations.Add(cv);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync()
        {
            var q = from cv in _db.ShoeColorVariations
                    join s in _db.Shoes on cv.ShoeId equals s.Id
                    select new InventoryReportDto
                    {
                        ShoeColorVariationId = cv.Id,
                        ShoeName = s.Name,
                        ColorName = cv.ColorName,
                        StockQuantity = cv.StockQuantity,
                        ReorderLevel = cv.ReorderLevel
                    };

            return await q.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<InventoryReportDto>> GetLowStockAsync()
        {
            return (await GetInventoryReportAsync()).Where(x => x.IsLowStock);
        }

        // ✅ FIX: Implementation of the missing interface member
        public async Task<IEnumerable<object>> GetAllShoeVariationsAsync()
        {
            // Query to select the variation ID and create a display name (Shoe Name - Color)
            return await _db.ShoeColorVariations
                .AsNoTracking()
                // Include the Shoe entity to access the Shoe Name property
                .Include(cv => cv.Shoe)
                .Select(cv => new
                {
                    // The ID is the value used in the form submission
                    Id = cv.Id,
                    // The display name for the dropdown
                    Name = cv.Shoe.Name + " - " + cv.ColorName
                })
                .ToListAsync();
        }

        private static ShoeDto ToDto(Shoe s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            Brand = s.Brand,
            Cost = s.Cost,
            Price = s.Price,
            Description = s.Description,
            ImageUrl = s.ImageUrl,
            IsActive = s.IsActive,
            CreatedDate = s.CreatedDate
        };
    }
}