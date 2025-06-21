namespace TossErp.POS.Application.DTOs;

public class UpdateSaleDto
{
    public int Id { get; set; }
    public List<UpdateSaleItemDto> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = "";
    public decimal DiscountAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
}

public class UpdateSaleItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
} 
