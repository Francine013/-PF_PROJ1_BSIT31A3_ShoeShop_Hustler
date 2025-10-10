namespace ShoeShop.Web.Models.ViewModels
{
    public class ReportViewModel
    {
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Revenue { get; set; }
    }
}
