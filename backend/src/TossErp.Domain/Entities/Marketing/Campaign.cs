using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Marketing;

/// <summary>
/// Marketing campaign management
/// </summary>
public class Campaign : BaseEntity
{
    public string CampaignNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CampaignType Type { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    
    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Budget
    public decimal Budget { get; set; }
    public decimal ActualSpend { get; set; }
    
    // Targeting
    public string? TargetAudience { get; set; }
    public string? SegmentIds { get; set; } // JSON array
    
    // Metrics
    public int TargetCount { get; set; }
    public int SentCount { get; set; }
    public int OpenCount { get; set; }
    public int ClickCount { get; set; }
    public int ConversionCount { get; set; }
    public decimal Revenue { get; set; }
    
    // Content
    public string? Subject { get; set; }
    public string? MessageBody { get; set; }
    
    // Metadata
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? Notes { get; set; }
}

public enum CampaignType
{
    Email,
    SMS,
    Social,
    Mixed
}

public enum CampaignStatus
{
    Draft,
    Scheduled,
    Active,
    Paused,
    Completed,
    Cancelled
}
