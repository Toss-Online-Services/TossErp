namespace Toss.Domain.Entities;

public class Shop : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public Location Location { get; set; } = null!;
    public int? AddressId { get; set; }
    public Address? Address { get; set; }
    public PhoneNumber? ContactPhone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Settings
    public string Currency { get; set; } = "ZAR";
    public decimal TaxRate { get; set; } = 15m; // 15% VAT
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "Africa/Johannesburg";
    public string? AreaGroup { get; set; }
    
    // Business hours
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    
    // Features
    public bool WhatsAppAlertsEnabled { get; set; } = true;
    public bool GroupBuyingEnabled { get; set; } = true;
    public bool AIAssistantEnabled { get; set; } = true;
}

