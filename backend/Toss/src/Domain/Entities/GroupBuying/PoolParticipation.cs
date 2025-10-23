namespace Toss.Domain.Entities.GroupBuying;

public class PoolParticipation : BaseAuditableEntity
{
    public int GroupBuyPoolId { get; set; }
    public GroupBuyPool GroupBuyPool { get; set; } = null!;
    
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int QuantityCommitted { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingShare { get; set; }
    public decimal Total { get; set; }
    
    public DateTimeOffset JoinedDate { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTimeOffset? ConfirmedDate { get; set; }
    
    public string? Notes { get; set; }
}

