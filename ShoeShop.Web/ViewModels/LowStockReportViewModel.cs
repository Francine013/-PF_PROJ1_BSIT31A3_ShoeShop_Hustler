namespace ShoeShop.Web.Models.ViewModels
{
    public class LowStockReportViewModel
    {
        public string id {  get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}
