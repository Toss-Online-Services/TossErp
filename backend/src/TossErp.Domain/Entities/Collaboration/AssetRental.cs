using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Rental transaction for a shared asset
/// </summary>
public class AssetRental : BaseEntity
{
    public string RentalNumber { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    
    // Renter
    public int RenterId { get; set; }
    public string RenterName { get; set; } = string.Empty;
    public string? RenterContact { get; set; }
    
    // Rental Period
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public int DurationDays { get; set; }
    public decimal ActualDurationHours { get; set; }
    
    // Pricing
    public decimal RatePerDay { get; set; }
    public decimal SecurityDeposit { get; set; }
    public decimal RentalAmount { get; set; }
    public decimal? LateFee { get; set; }
    public decimal? DamageFee { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Payment
    public bool IsDepositPaid { get; set; }
    public bool IsRentalPaid { get; set; }
    public bool IsDepositReturned { get; set; }
    public DateTime? DepositReturnedDate { get; set; }
    
    // Status
    public RentalStatus Status { get; set; } = RentalStatus.Requested;
    
    // Condition
    public string? ConditionAtPickup { get; set; }
    public string? ConditionAtReturn { get; set; }
    public bool HasDamage { get; set; }
    public string? DamageDescription { get; set; }
    public string? DamagePhotos { get; set; } // JSON array of URLs
    
    // Review
    public int? RenterRating { get; set; } // 1-5 by owner
    public string? RenterReview { get; set; }
    public int? AssetRating { get; set; } // 1-5 by renter
    public string? AssetReview { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    
    // Navigation Properties
    public SharedAsset Asset { get; set; } = null!;
    
    // Business Methods
    public void Approve()
    {
        if (Status != RentalStatus.Requested)
            throw new InvalidOperationException("Can only approve requested rentals");
        
        Status = RentalStatus.Approved;
    }
    
    public void StartRental()
    {
        if (Status != RentalStatus.Approved)
            throw new InvalidOperationException("Rental must be approved");
        
        Status = RentalStatus.Active;
        AddDomainEvent(new AssetRented(Id, AssetId, RenterId, StartDate, EndDate));
    }
    
    public void CompleteRental(string condition, bool hasDamage)
    {
        Status = RentalStatus.Completed;
        ActualReturnDate = DateTime.UtcNow;
        ConditionAtReturn = condition;
        HasDamage = hasDamage;
    }
}

public enum RentalStatus
{
    Requested,
    Approved,
    Active,
    Completed,
    Cancelled,
    Overdue
}

