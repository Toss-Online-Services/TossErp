namespace TossErp.POS.Application.DTOs;

public class SaleItemDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
} 
