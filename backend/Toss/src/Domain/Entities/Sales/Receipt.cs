namespace Toss.Domain.Entities.Sales;

public class Receipt : BaseAuditableEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    
    public DateTimeOffset IssuedDate { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    
    // For printing/display
    public string? ReceiptContent { get; set; } // JSON or text format
    public bool IsPrinted { get; set; }
    public DateTimeOffset? PrintedAt { get; set; }
}

