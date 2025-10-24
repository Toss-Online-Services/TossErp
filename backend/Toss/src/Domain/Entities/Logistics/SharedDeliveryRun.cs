namespace Toss.Domain.Entities.Logistics;

public class SharedDeliveryRun : BaseAuditableEntity
{
    public string RunNumber { get; set; } = string.Empty;
    
    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }
    
    public int? GroupBuyPoolId { get; set; }
    
    public DateTimeOffset ScheduledDate { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? AssignedDate { get; set; }
    
    // Aliases for handlers
    public DateTimeOffset? ActualDepartureTime
    {
        get => StartedAt;
        set => StartedAt = value;
    }
    
    public DateTimeOffset? ActualArrivalTime
    {
        get => CompletedAt;
        set => CompletedAt = value;
    }
    
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;
    
    public string? AreaGroup { get; set; }
    public Location? StartLocation { get; set; }
    
    // Costs and metrics
    public decimal TotalDeliveryCost { get; set; }
    public decimal TotalDistance { get; set; }
    public int ParticipantCount { get; set; }
    public decimal CostPerStop { get; set; }
    
    // Alias for handlers
    public decimal TotalCost
    {
        get => TotalDeliveryCost;
        set => TotalDeliveryCost = value;
    }
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<DeliveryStop> Stops { get; private set; } = new List<DeliveryStop>();
}

