using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Procurement;

public enum SupplierType
{
    Individual,
    Company
}

public enum SupplierStatus
{
    Active,
    Inactive,
    Suspended,
    Blocked
}

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public SupplierType Type { get; set; } = SupplierType.Company;
    public SupplierStatus Status { get; set; } = SupplierStatus.Active;
    
    // Contact information
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    
    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "South Africa";
    
    // Business details
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    
    // Financial
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    
    // Performance tracking
    public decimal? AverageLeadTimeDays { get; set; }
    public decimal? QualityRating { get; set; } // 0-5 stars
    public decimal? DeliveryRating { get; set; } // 0-5 stars
    public int TotalOrders { get; set; }
    
    // Contact person
    public string? ContactPersonName { get; set; }
    public string? ContactPersonPhone { get; set; }
    public string? ContactPersonEmail { get; set; }
    
    // Navigation properties
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    
    // Business logic
    public void Activate()
    {
        Status = SupplierStatus.Active;
        AddDomainEvent(new SupplierActivatedEvent(Id, Name));
    }
    
    public void Suspend(string reason)
    {
        Status = SupplierStatus.Suspended;
        AddDomainEvent(new SupplierSuspendedEvent(Id, Name, reason));
    }
    
    public void UpdateRating(decimal quality, decimal delivery)
    {
        QualityRating = quality;
        DeliveryRating = delivery;
        AddDomainEvent(new SupplierRatingUpdatedEvent(Id, Name, quality, delivery));
    }
}

public enum PurchaseOrderStatus
{
    Draft,
    Submitted,
    Approved,
    Ordered,
    PartiallyReceived,
    Received,
    Cancelled
}

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; } = null!;
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    
    public int? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    
    // Business logic
    public void Submit()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted");
        
        Status = PurchaseOrderStatus.Submitted;
        AddDomainEvent(new PurchaseOrderSubmittedEvent(Id, OrderNumber));
    }
    
    public void Approve(string approvedBy)
    {
        if (Status != PurchaseOrderStatus.Submitted)
            throw new InvalidOperationException("Only submitted orders can be approved");
        
        Status = PurchaseOrderStatus.Approved;
        ApprovedByName = approvedBy;
        ApprovedDate = DateTime.UtcNow;
        AddDomainEvent(new PurchaseOrderApprovedEvent(Id, OrderNumber, approvedBy));
    }
    
    public void CalculateTotals(decimal taxRate)
    {
        Subtotal = Items.Sum(i => i.LineTotal);
        TaxAmount = Subtotal * taxRate;
        TotalAmount = Subtotal + TaxAmount + ShippingCost;
    }
}

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    
    public string? Notes { get; set; }
}

// Domain Events
public class SupplierActivatedEvent : DomainEvent
{
    public int SupplierId { get; }
    public string SupplierName { get; }
    
    public SupplierActivatedEvent(int supplierId, string supplierName)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
    }
}

public class SupplierSuspendedEvent : DomainEvent
{
    public int SupplierId { get; }
    public string SupplierName { get; }
    public string Reason { get; }
    
    public SupplierSuspendedEvent(int supplierId, string supplierName, string reason)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        Reason = reason;
    }
}

public class SupplierRatingUpdatedEvent : DomainEvent
{
    public int SupplierId { get; }
    public string SupplierName { get; }
    public decimal QualityRating { get; }
    public decimal DeliveryRating { get; }
    
    public SupplierRatingUpdatedEvent(int supplierId, string supplierName, decimal qualityRating, decimal deliveryRating)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        QualityRating = qualityRating;
        DeliveryRating = deliveryRating;
    }
}

public class PurchaseOrderSubmittedEvent : DomainEvent
{
    public int PurchaseOrderId { get; }
    public string OrderNumber { get; }
    
    public PurchaseOrderSubmittedEvent(int purchaseOrderId, string orderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        OrderNumber = orderNumber;
    }
}

public class PurchaseOrderApprovedEvent : DomainEvent
{
    public int PurchaseOrderId { get; }
    public string OrderNumber { get; }
    public string ApprovedBy { get; }
    
    public PurchaseOrderApprovedEvent(int purchaseOrderId, string orderNumber, string approvedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        OrderNumber = orderNumber;
        ApprovedBy = approvedBy;
    }
}

