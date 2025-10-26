namespace Toss.Domain.Entities.GroupBuying;

public class PoolParticipation : BaseAuditableEntity
{
    public int GroupBuyPoolId { get; set; }
    public GroupBuyPool GroupBuyPool { get; set; } = null!;
    
    // Alias for handlers
    public int PoolId
    {
        get => GroupBuyPoolId;
        set => GroupBuyPoolId = value;
    }
    public GroupBuyPool Pool => GroupBuyPool;
    
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;
    
    public int QuantityCommitted { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingShare { get; set; }
    public decimal Total { get; set; }
    
    // Alias for handlers
    public int Quantity => QuantityCommitted;
    
    public DateTimeOffset JoinedDate { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTimeOffset? ConfirmedDate { get; set; }
    
    public string? Notes { get; set; }
}

