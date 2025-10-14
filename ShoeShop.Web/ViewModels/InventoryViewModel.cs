using ShoeShop.Services.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace ShoeShop.Web.ViewModels
{
    public class InventoryViewModel
    {
        public IEnumerable<ShoeDto> Shoes { get; set; } = new List<ShoeDto>();
        public IEnumerable<InventoryReportDto> InventoryReports { get; set; } = new List<InventoryReportDto>();

        public IEnumerable<ShoeWithStockViewModel> ShoesWithStock
        {
            get
            {
                return Shoes.GroupJoin(
                    InventoryReports,
                    shoe => shoe.Id,
                    report => report.ShoeColorVariationId,
                    (shoe, reports) => new ShoeWithStockViewModel
                    {
                        Id = shoe.Id,
                        Name = shoe.Name,
                        Brand = shoe.Brand,
                        Price = shoe.Price,
                        ImageUrl = shoe.ImageUrl,
                        StockQuantity = reports.Sum(r => r.StockQuantity),
                        IsActive = shoe.IsActive
                    }
                );
            }
        }
    }

    public class ShoeWithStockViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}
