namespace ShoeShop.Services.DTOs
{
    public class ShoeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }

        public string AvailabilityStatus { get; set; } = "Available";
        public int StockQuantity { get; set; }
        public string ColorName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int size { get; set; }

    }
}
