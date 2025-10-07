using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.SupplyChain;

/// <summary>
/// Shipping carrier/logistics provider
/// </summary>
public class Carrier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public CarrierType Type { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Contact Information
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    
    // Service Details
    public string? ServiceTypes { get; set; } // JSON array: Express, Standard, Economy
    public string? CoverageAreas { get; set; } // JSON array of covered regions
    public decimal? MinimumShipmentValue { get; set; }
    public decimal? MaximumWeight { get; set; }
    
    // Performance Metrics
    public decimal OnTimeDeliveryRate { get; set; } // Percentage
    public decimal DamageRate { get; set; } // Percentage
    public int TotalShipments { get; set; }
    public decimal AverageRating { get; set; } // 1-5 stars
    
    // Pricing
    public string? RateStructure { get; set; } // JSON pricing tiers
    public string? Currency { get; set; } = "ZAR";
    
    // Integration
    public bool HasApiIntegration { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ApiKey { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
}

public enum CarrierType
{
    Courier,
    FreightForwarder,
    OwnFleet,
    ThirdParty,
    Postal
}
