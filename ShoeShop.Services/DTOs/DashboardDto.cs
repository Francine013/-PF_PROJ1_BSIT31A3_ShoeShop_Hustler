using System;
using System.Collections.Generic;

namespace ShoeShop.Services.DTOs
{
    public class DashboardDto
    {
        public int TotalShoes { get; set; }
        public int TotalColorVariations { get; set; }
        public int LowStockItems { get; set; }
        public int PendingPullOuts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<StockAlertDto> StockAlerts { get; set; } = new List<StockAlertDto>();
        public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
    }

    public class StockAlertDto
    {
        public string ShoeName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
    }

    public class RecentActivityDto
    {
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
