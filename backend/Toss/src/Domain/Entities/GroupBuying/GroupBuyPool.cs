namespace Toss.Domain.Entities.GroupBuying;

public class GroupBuyPool : BaseAuditableEntity
{
    public string PoolNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int InitiatorShopId { get; set; }
    public Shop InitiatorShop { get; set; } = null!;
    
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    
    // Pooling details
    public int MinimumQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal BulkDiscountPercentage { get; set; }
    public decimal FinalUnitPrice { get; set; }
    
    // Timing
    public DateTimeOffset OpenDate { get; set; }
    public DateTimeOffset CloseDate { get; set; }
    public DateTimeOffset? ConfirmedDate { get; set; }
    
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

