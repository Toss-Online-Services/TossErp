using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Sales;

public class Sale : BaseAuditableEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;
    
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public DateTimeOffset SaleDate { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    
    /// <summary>
    /// Type of sale - POS, QueueOrder, Delivery, PreOrder
    /// </summary>
    public SaleType SaleType { get; set; } = SaleType.POS;
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    
    // Alias for Total (used by handlers)
    public decimal TotalAmount
    {
        get => Total;
        set => Total = value;
    }
    
    // Payment
    public PaymentType PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    
    // Void tracking
    public string? VoidReason { get; set; }
    public DateTimeOffset? VoidedAt { get; set; }
    
    public string? Notes { get; set; }
    
    // Queue-based order fields
    /// <summary>
    /// Expected time when order will be ready (for queue orders)
    /// </summary>
    public DateTimeOffset? ExpectedCompletionTime { get; set; }
    
    /// <summary>
    /// Position in the preparation queue
    /// </summary>
    public int? QueuePosition { get; set; }
    
    /// <summary>
    /// Customer-specific notes for order preparation
    /// </summary>
    public string? CustomerNotes { get; set; }
    
    /// <summary>
    /// Customer name for walk-in orders (when CustomerId is null)
    /// </summary>
    public string? CustomerName { get; set; }
    
    /// <summary>
    /// Customer phone for notifications
    /// </summary>
    public string? CustomerPhone { get; set; }
    
    // Relationships
    public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();
    public ICollection<SalesDocument> Documents { get; set; } = new List<SalesDocument>();
}

