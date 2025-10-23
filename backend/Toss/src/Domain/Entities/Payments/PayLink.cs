namespace Toss.Domain.Entities.Payments;

public class PayLink : BaseAuditableEntity
{
    public string LinkCode { get; set; } = string.Empty;
    public string FullUrl { get; set; } = string.Empty;
    
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Description { get; set; } = string.Empty;
    
    // Source
    public string? SourceType { get; set; } // "Sale", "Invoice"
    public int? SourceId { get; set; }
    
    // Customer details
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsUsed { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    
    // Expiry
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    
    // Relationships
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
}

