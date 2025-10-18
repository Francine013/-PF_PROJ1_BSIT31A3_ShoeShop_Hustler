using System.Collections.Generic;

namespace ShoeShop.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalShoes { get; set; }
        public int TotalColorVariations { get; set; }
        public int LowStockItems { get; set; }
        public int PendingPullOuts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<StockAlert> StockAlerts { get; set; } = new List<StockAlert>();
        public List<RecentActivity> RecentActivities { get; set; } = new List<RecentActivity>();
    }
    public class StockAlert
    {
        public string ShoeName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
    }
    public class RecentActivity
    {
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public System.DateTime Timestamp { get; set; } 
    }
}
