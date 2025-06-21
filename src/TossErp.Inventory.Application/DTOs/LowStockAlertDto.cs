namespace TossErp.Inventory.Application.DTOs;

public class LowStockAlertDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public DateTime AlertDate { get; set; }
} 
