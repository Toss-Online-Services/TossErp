namespace TossErp.POS.Application.DTOs;

public class SaleListDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = "";
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string Status { get; set; } = "";
    public string? CustomerName { get; set; }
    public int ItemCount { get; set; }
} 
