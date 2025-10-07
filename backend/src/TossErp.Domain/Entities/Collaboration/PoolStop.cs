using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

public class PoolStop : BaseEntity
{
    public int DeliveryPoolId { get; set; }
    public int Sequence { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public int ParticipantCount { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
    
    // Navigation
    public DeliveryPool DeliveryPool { get; set; } = null!;
}

