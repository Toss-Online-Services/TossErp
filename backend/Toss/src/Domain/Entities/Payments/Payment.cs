namespace Toss.Domain.Entities.Payments;

public class Payment : BaseAuditableEntity
{
    public string PaymentReference { get; set; } = string.Empty;
    
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    
    // Source
    public string? SourceType { get; set; } // "Sale", "PurchaseOrder", "GroupBuy"
    public int? SourceId { get; set; }
    
    // Explicit source references (for handler compatibility)
    public int? SaleId { get; set; }
    public int? PurchaseOrderId { get; set; }
    public string? TransactionRef { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    
    // Customer/Payer details
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string? PayerName { get; set; }
    public string? PayerPhone { get; set; }
    public string? PayerEmail { get; set; }
    
    // Gateway details
    public string? GatewayReference { get; set; }
    public string? GatewayResponse { get; set; }
    
    // Timing
    public DateTimeOffset InitiatedAt { get; set; }
    public DateTimeOffset? AuthorizedAt { get; set; }
    public DateTimeOffset? CapturedAt { get; set; }
    public DateTimeOffset? FailedAt { get; set; }
    
    public string? FailureReason { get; set; }
    public string? Notes { get; set; }
    
    // Pay link reference
    public int? PayLinkId { get; set; }
    public PayLink? PayLink { get; set; }
}

