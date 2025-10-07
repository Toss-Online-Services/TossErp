using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

public class DeliveryPoolParticipant : BaseEntity
{
    public int DeliveryPoolId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    
    // Stop Details
    public int StopSequence { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryInstructions { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Package Details
    public int PackageCount { get; set; }
    public decimal? TotalWeight { get; set; }
    public decimal? TotalVolume { get; set; }
    
    // Cost
    public decimal CostShare { get; set; }
    public bool HasPaid { get; set; }
    
    // Status
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ReceivedBy { get; set; }
    
    // Navigation
    public DeliveryPool DeliveryPool { get; set; } = null!;
}

