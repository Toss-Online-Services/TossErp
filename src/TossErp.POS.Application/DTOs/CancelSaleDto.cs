namespace TossErp.POS.Application.DTOs;

public class CancelSaleDto
{
    public int SaleId { get; set; }
    public string Reason { get; set; } = "";
    public string? CancelledBy { get; set; }
} 
