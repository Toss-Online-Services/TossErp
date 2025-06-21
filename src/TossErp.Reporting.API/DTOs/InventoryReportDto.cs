namespace TossErp.Reporting.API.DTOs
{
    public class InventoryReportDto
    {
        public DateTime ReportDate { get; set; }
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<InventoryReportItemDto> LowStockAlerts { get; set; } = new();
        public List<InventoryReportItemDto> TopValueItems { get; set; } = new();
    }

    public class InventoryReportItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
    }
} 
