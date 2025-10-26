namespace Toss.Domain.Entities.Logistics;

public class DeliveryStop : BaseAuditableEntity
{
    public int SharedDeliveryRunId { get; set; }
    public SharedDeliveryRun SharedDeliveryRun { get; set; } = null!;
    
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;
    
    public int SequenceNumber { get; set; }
    
    public Location DeliveryLocation { get; set; } = null!;
    public string? DeliveryInstructions { get; set; }
    
    public decimal CostShare { get; set; }
    
    public DateTimeOffset? ArrivalTime { get; set; }
    public DateTimeOffset? CompletionTime { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;
    
    // Alias for handlers
    public DateTimeOffset? ActualDeliveryTime
    {
        get => CompletionTime;
        set => CompletionTime = value;
    }
    
    // References
    public int? PurchaseOrderId { get; set; }
    public int? PoolParticipationId { get; set; }
    
    // POD - Collection for handlers
    public ICollection<ProofOfDelivery> ProofOfDeliveries { get; private set; } = new List<ProofOfDelivery>();
    
    public string? Notes { get; set; }
}

