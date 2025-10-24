namespace Toss.Domain.Entities.Logistics;

public class Driver : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    // Alias for handlers
    public string Name => FullName;
    
    public PhoneNumber Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? LicenseNumber { get; set; }
    
    public string? VehicleType { get; set; }
    public string? VehicleRegistration { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    
    // Relationships
    public ICollection<SharedDeliveryRun> DeliveryRuns { get; private set; } = new List<SharedDeliveryRun>();
}

