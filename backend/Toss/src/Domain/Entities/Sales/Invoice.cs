namespace Toss.Domain.Entities.Sales;

public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public DateTimeOffset InvoiceDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    
    public bool IsPaid { get; set; }
    public DateTimeOffset? PaidDate { get; set; }
    
    public string? Notes { get; set; }
}

