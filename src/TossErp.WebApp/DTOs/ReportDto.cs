namespace TossErp.WebApp.DTOs
{
    public class InventoryReportDto
    {
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal TotalValue { get; set; }
        public List<InventoryReportItemDto> Items { get; set; } = new();
    }

    public class InventoryReportItemDto
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public int StockLevel { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public decimal UnitCost { get; set; }
        public decimal StandardCost { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class SalesReportDto
    {
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string TopProduct { get; set; } = string.Empty;
        public List<SalesReportItemDto> Items { get; set; } = new();
        public decimal TotalSales { get; set; }
    }

    public class SalesReportItemDto
    {
        public Guid Id { get; set; }
        public string PeriodOrProduct { get; set; } = string.Empty;
        public decimal SalesAmount { get; set; }
        public int Orders { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class FinancialReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal NetProfit { get; set; }
        public List<FinancialReportItemDto> Items { get; set; } = new();
    }

    public class FinancialReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
    }
} 
