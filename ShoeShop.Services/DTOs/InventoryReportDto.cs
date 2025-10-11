namespace ShoeShop.Services.DTOs
{
    public class InventoryReportDto
    {
        public int ShoeColorVariationId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = "N/A";
        public string ColorName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Revenue { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsLowStock => StockQuantity <= ReorderLevel;
    }
}
