namespace ShoeShop.Web.ViewModels
{
    public class PullOutsViewModel
    {
        public int Id { get; set; }

        public int ShoeColorVariationId { get; set; }

        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetails { get; set; }

        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? ShoeName { get; set; }
        public string? ColorName { get; set; }
        public string? BrandName { get; set; }
    }
}
