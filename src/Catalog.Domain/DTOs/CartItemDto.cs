namespace Catalog.Domain.DTOs;

public class CartItemDto
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public CatalogItemDto? CatalogItem { get; set; }
} 
