namespace TossErp.POS.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = "";
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string Status { get; set; } = "";
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
} 
