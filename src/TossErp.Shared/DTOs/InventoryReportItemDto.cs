namespace TossErp.Shared.DTOs;

public class InventoryReportItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public int StockLevel { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
} 
