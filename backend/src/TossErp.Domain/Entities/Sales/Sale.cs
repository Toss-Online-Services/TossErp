using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Sales;

public enum SaleStatus
{
    Draft,
    Completed,
    Cancelled,
    Refunded,
    PartiallyRefunded
}

public enum SaleType
{
    Regular,
    Return,
    Exchange
}

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public SaleStatus Status { get; set; } = SaleStatus.Draft;
    public SaleType Type { get; set; } = SaleType.Regular;
    
    // Customer information
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    
    // Financial details (stored in cents)
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Location and user tracking
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public int? CashierId { get; set; }
    public string? CashierName { get; set; }
    public string? PosDeviceId { get; set; }
    public string? PosDeviceName { get; set; }
    
    // Transaction details
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string? ReceiptNumber { get; set; }
    public bool IsPrinted { get; set; }
    public bool IsEmailSent { get; set; }
    
    // Loyalty
    public int? LoyaltyPointsEarned { get; set; }
    public int? LoyaltyPointsRedeemed { get; set; }
    
    // Navigation properties
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    // Business logic
    public void Complete()
    {
        if (Status != SaleStatus.Draft)
            throw new InvalidOperationException("Only draft sales can be completed");
        
        if (Items.Count == 0)
            throw new InvalidOperationException("Cannot complete sale with no items");
        
        Status = SaleStatus.Completed;
        AddDomainEvent(new SaleCompletedEvent(Id, SaleNumber, TotalAmount));
    }
    
    public void Cancel(string reason)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");
        
        Status = SaleStatus.Cancelled;
        Notes = $"{Notes}\nCancelled: {reason}";
        AddDomainEvent(new SaleCancelledEvent(Id, SaleNumber, reason));
    }
    
    public void CalculateTotals(decimal taxRate)
    {
        Subtotal = Items.Sum(i => i.LineTotal);
        TaxAmount = (Subtotal - DiscountAmount) * taxRate;
        TotalAmount = Subtotal - DiscountAmount + TaxAmount;
    }
}

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }
    public virtual Sale Sale { get; set; } = null!;
    
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    
    public string? Notes { get; set; }
    
    public void CalculateLineTotal()
    {
        LineTotal = (UnitPrice * Quantity) - Discount + TaxAmount;
    }
}

// Domain Events
public class SaleCompletedEvent : DomainEvent
{
    public int SaleId { get; }
    public string SaleNumber { get; }
    public decimal TotalAmount { get; }
    
    public SaleCompletedEvent(int saleId, string saleNumber, decimal totalAmount)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        TotalAmount = totalAmount;
    }
}

public class SaleCancelledEvent : DomainEvent
{
    public int SaleId { get; }
    public string SaleNumber { get; }
    public string Reason { get; }
    
    public SaleCancelledEvent(int saleId, string saleNumber, string reason)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        Reason = reason;
    }
}

