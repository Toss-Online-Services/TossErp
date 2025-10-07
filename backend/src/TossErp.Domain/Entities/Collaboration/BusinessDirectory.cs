using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Community business directory listing
/// </summary>
public class BusinessDirectory : BaseEntity
{
    public int CustomerId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? TradingName { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    
    // Contact
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    
    // Location
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Services
    public string? ServicesOffered { get; set; } // JSON array
    public string? ProductsOffered { get; set; } // JSON array
    
    // Social
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    
    // Media
    public string? LogoUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public string? GalleryPhotos { get; set; } // JSON array
    
    // Stats
    public int ViewCount { get; set; }
    public int ContactClickCount { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    // Status
    public bool IsVerified { get; set; }
    public bool IsPremium { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? VerifiedDate { get; set; }
    
    // Hours
    public string? OperatingHours { get; set; } // JSON: {monday: "8-17", ...}
    
    // Metadata
    public string? Tags { get; set; } // JSON array
    public string? Notes { get; set; }
}

