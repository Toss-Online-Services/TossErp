namespace TossErp.POS.Application.DTOs;

public class CreateSaleDto
{
    public List<CreateSaleItemDto> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = "";
    public decimal DiscountAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
}

public class CreateSaleItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
} 
