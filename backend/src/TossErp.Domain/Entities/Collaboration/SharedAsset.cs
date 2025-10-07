using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Asset available for rental/sharing in the community
/// </summary>
public class SharedAsset : BaseEntity
{
    public string AssetNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssetCategory Category { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.Available;
    
    // Owner
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerContact { get; set; }
    
    // Asset Details
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearPurchased { get; set; }
    public string? Condition { get; set; } // New, Good, Fair, Poor
    
    // Location
    public string? LocationAddress { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Rental Pricing
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? SecurityDeposit { get; set; }
    
    // Availability
    public bool IsAvailable { get; set; } = true;
    public string? AvailabilitySchedule { get; set; } // JSON calendar
    public int? MinimumRentalHours { get; set; }
    public int? MaximumRentalDays { get; set; }
    
    // Usage Stats
    public int TotalRentals { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    // Maintenance
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? MaintenanceNotes { get; set; }
    
    // Terms
    public string? RentalTerms { get; set; }
    public string? InsuranceRequired { get; set; }
    public string? OperatorRequired { get; set; } // Yes, No, Optional
    
    // Media
    public string? Photos { get; set; } // JSON array of URLs
    public string? Documents { get; set; } // JSON array of document URLs
    
    // Metadata
    public string? Notes { get; set; }
    public string? Tags { get; set; } // JSON array
    
    // Navigation Properties
    public ICollection<AssetRental> Rentals { get; set; } = new List<AssetRental>();
    
    // Business Methods
    public void ListForRent()
    {
        if (Status != AssetStatus.Available)
            throw new InvalidOperationException("Asset must be available to list");
        
        Status = AssetStatus.Listed;
        IsAvailable = true;
        AddDomainEvent(new AssetListedForRent(Id, Name, OwnerId, DailyRate));
    }
}

public enum AssetCategory
{
    Vehicles,
    Equipment,
    Tools,
    Machinery,
    Technology,
    Property,
    Other
}

public enum AssetStatus
{
    Available,
    Listed,
    Rented,
    Maintenance,
    Retired
}

