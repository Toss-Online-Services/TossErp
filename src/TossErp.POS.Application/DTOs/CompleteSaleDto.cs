namespace TossErp.POS.Application.DTOs;

public class CompleteSaleDto
{
    public int SaleId { get; set; }
    public string PaymentMethod { get; set; } = "";
    public decimal AmountPaid { get; set; }
    public decimal ChangeAmount { get; set; }
    public string? ReceiptEmail { get; set; }
    public string? ReceiptPhone { get; set; }
} 
