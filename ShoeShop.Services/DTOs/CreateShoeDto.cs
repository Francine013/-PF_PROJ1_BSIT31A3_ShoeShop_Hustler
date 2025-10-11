using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreateShoeDto
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public int size { get; set; }
        public string ColorName { get; set; }
        public string AvailabilityStatus { get; set; } = "Available";
        public bool IsActive { get; set; } = true;
    }
}
