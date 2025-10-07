using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Ecommerce;

/// <summary>
/// Online order from e-commerce platform
/// </summary>
public class OnlineOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public string PlatformOrderId { get; set; } = string.Empty; // External platform ID
    public string Platform { get; set; } = string.Empty; // Shopify, WooCommerce, etc.
    public OnlineOrderStatus Status { get; set; } = OnlineOrderStatus.Pending;
    
    // Customer
    public int? CustomerId { get; set; } // Linked internal customer
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Dates
    public DateTime OrderDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    
    // Shipping
    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? TrackingNumber { get; set; }
    
    // Payment
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    
    // Sync
    public bool IsSynced { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public int? LinkedSalesOrderId { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? RawData { get; set; } // Store full JSON from platform
}

public enum OnlineOrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
