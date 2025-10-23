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
    
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;
    
    public string? AreaGroup { get; set; }
    public Location? StartLocation { get; set; }
    
    // Costs
    public decimal TotalDeliveryCost { get; set; }
    public int ParticipantCount { get; set; }
    public decimal CostPerStop { get; set; }
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<DeliveryStop> Stops { get; private set; } = new List<DeliveryStop>();
}

