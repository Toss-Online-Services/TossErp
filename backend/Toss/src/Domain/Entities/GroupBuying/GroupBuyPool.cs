namespace Toss.Domain.Entities.GroupBuying;

public class GroupBuyPool : BaseAuditableEntity
{
    public string PoolNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int InitiatorShopId { get; set; }
    public Store InitiatorShop { get; set; } = null!;
    
    // Alias for handlers
    public int CreatorShopId => InitiatorShopId;
    
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    
    // Pooling details
    public int MinimumQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public int TargetParticipants { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal BulkDiscountPercentage { get; set; }
    public decimal FinalUnitPrice { get; set; }
    
    // Aliases for handlers
    public decimal ProductPrice => UnitPrice;
    
    // Timing
    public DateTimeOffset OpenDate { get; set; }
    public DateTimeOffset CloseDate { get; set; }
    public DateTimeOffset? ConfirmedDate { get; set; }
    
    // Alias for handlers
    public DateTimeOffset TargetDate => CloseDate;
    
    public PoolStatus Status { get; set; } = PoolStatus.Open;
    
    // Location/Area filtering
    public string? AreaGroup { get; set; }
    public double? MaxDistanceKm { get; set; }
    
    // Delivery
    public decimal EstimatedShippingCost { get; set; }
    public decimal? ActualShippingCost { get; set; }
    public int? SharedDeliveryRunId { get; set; }
    
    // Relationships
    public ICollection<PoolParticipation> Participations { get; private set; } = new List<PoolParticipation>();
    public int? AggregatedPurchaseOrderId { get; set; }
    public AggregatedPurchaseOrder? AggregatedPurchaseOrder { get; set; }
}

