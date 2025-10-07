using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Shared delivery pool for cost-effective logistics
/// </summary>
public class DeliveryPool : BaseEntity
{
    public string PoolNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PoolStatus Status { get; set; } = PoolStatus.Forming;
    
    // Route Information
    public string? OriginArea { get; set; }
    public string? DestinationArea { get; set; }
    public string? RouteDescription { get; set; }
    public decimal? DistanceKm { get; set; }
    
    // Organizer
    public int OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    
    // Scheduling
    public DateTime ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public DateTime? ActualDepartureTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    
    // Capacity
    public int MinimumParticipants { get; set; } = 3;
    public int MaximumParticipants { get; set; } = 10;
    public int CurrentParticipants { get; set; }
    public decimal? MaximumWeight { get; set; }
    public decimal? MaximumVolume { get; set; }
    
    // Costs
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostPerParticipant { get; set; }
    public decimal SavingsPerParticipant { get; set; }
    
    // Vehicle
    public string? VehicleType { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    
    // Metadata
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly
    public string? Notes { get; set; }
    
    // Navigation Properties
    public ICollection<DeliveryPoolParticipant> Participants { get; set; } = new List<DeliveryPoolParticipant>();
    public ICollection<PoolStop> Stops { get; set; } = new List<PoolStop>();
    
    // Business Methods
    public void Activate()
    {
        if (CurrentParticipants < MinimumParticipants)
            throw new InvalidOperationException($"Need at least {MinimumParticipants} participants");
        
        Status = PoolStatus.Active;
        AddDomainEvent(new DeliveryPoolCreated(Id, PoolNumber, CurrentParticipants));
    }
    
    public void AddParticipant(int customerId, string customerName, decimal costShare)
    {
        if (CurrentParticipants >= MaximumParticipants)
            throw new InvalidOperationException("Pool is full");
        
        CurrentParticipants++;
        CostPerParticipant = EstimatedCost / CurrentParticipants;
    }
}

public enum PoolStatus
{
    Forming,
    Active,
    InTransit,
    Completed,
    Cancelled
}

