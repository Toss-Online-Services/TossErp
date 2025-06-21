namespace TossErp.Inventory.Application.DTOs;

public class StockItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = "";
    public string Category { get; set; } = "";
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
} 
