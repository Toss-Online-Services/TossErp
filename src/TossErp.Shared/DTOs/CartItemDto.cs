namespace TossErp.Shared.DTOs;

public class CartItemDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int AvailableStock { get; set; }
} 
