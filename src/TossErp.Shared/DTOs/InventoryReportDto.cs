namespace TossErp.Shared.DTOs;

public class InventoryReportDto
{
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
    public List<InventoryItemDto> InventoryData { get; set; } = new();
    public List<InventoryReportItemDto> Items { get; set; } = new();
}

public class CategoryDistributionDto
{
    public string Category { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public double Percentage { get; set; }
}

public class InventoryItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
} 
