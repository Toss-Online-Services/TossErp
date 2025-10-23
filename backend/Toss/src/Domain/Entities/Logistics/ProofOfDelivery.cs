namespace Toss.Domain.Entities.Logistics;

public class ProofOfDelivery : BaseAuditableEntity
{
    public int DeliveryStopId { get; set; }
    public DeliveryStop DeliveryStop { get; set; } = null!;
    
    public ProofOfDeliveryType ProofType { get; set; }
    
    // PIN-based
    public string? PIN { get; set; }
    
    // Photo-based
    public string? PhotoUrl { get; set; }
    
    // Signature-based
    public string? SignatureData { get; set; }
    public string? SignedBy { get; set; }
    
    public DateTimeOffset CapturedAt { get; set; }
    public Location? CaptureLocation { get; set; }
    
    public string? Notes { get; set; }
}

