using Collaboration.Domain.Enums;

namespace Collaboration.Application.DTOs;

/// <summary>
/// Campaign data transfer object
/// </summary>
public class CampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; }
    public CampaignType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int MinParticipants { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool CanJoin { get; set; }
    public bool IsSuccessful { get; set; }
    public bool IsExpired { get; set; }
}

/// <summary>
/// Detailed campaign data transfer object with participants and quotations
/// </summary>
public class CampaignDetailsDto : CampaignDto
{
    public List<CampaignParticipantDto> Participants { get; set; } = new();
    public List<SupplierQuotationDto> Quotations { get; set; } = new();
    public List<CampaignAllocationDto> Allocations { get; set; } = new();
}

/// <summary>
/// Create campaign request DTO
/// </summary>
public class CreateCampaignDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CampaignType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinParticipants { get; set; }
    public int MaxParticipants { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid TenantId { get; set; }
}

/// <summary>
/// Update campaign request DTO
/// </summary>
public class UpdateCampaignDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Join campaign request DTO
/// </summary>
public class JoinCampaignDto
{
    public Guid UserId { get; set; }
    public decimal CommittedAmount { get; set; }
    public int DesiredQuantity { get; set; }
}

/// <summary>
/// Leave campaign request DTO
/// </summary>
public class LeaveCampaignDto
{
    public Guid UserId { get; set; }
}

/// <summary>
/// Cancel campaign request DTO
/// </summary>
public class CancelCampaignDto
{
    public string Reason { get; set; } = string.Empty;
    public Guid CancelledBy { get; set; }
}

/// <summary>
/// Campaign participant DTO
/// </summary>
public class CampaignParticipantDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public decimal CommittedAmount { get; set; }
    public int DesiredQuantity { get; set; }
    public ParticipantStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// Campaign allocation DTO
/// </summary>
public class CampaignAllocationDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public AllocationType Type { get; set; }
    public int AllocatedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public AllocationStatus Status { get; set; }
    public DateTime AllocatedAt { get; set; }
}

/// <summary>
/// Campaign analytics DTO
/// </summary>
public class CampaignAnalyticsDto
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public int TotalParticipants { get; set; }
    public decimal TotalCommittedAmount { get; set; }
    public decimal AverageCommittedAmount { get; set; }
    public decimal ProgressToTarget { get; set; }
    public int DaysRemaining { get; set; }
    public decimal ProjectedSavings { get; set; }
    public int TotalQuotations { get; set; }
    public decimal BestQuotationPrice { get; set; }
    public List<ParticipantAnalyticsDto> TopParticipants { get; set; } = new();
    public List<DailyParticipationDto> DailyParticipation { get; set; } = new();
}

/// <summary>
/// Participant analytics DTO
/// </summary>
public class ParticipantAnalyticsDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal CommittedAmount { get; set; }
    public int DesiredQuantity { get; set; }
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// Daily participation DTO for analytics
/// </summary>
public class DailyParticipationDto
{
    public DateTime Date { get; set; }
    public int NewParticipants { get; set; }
    public decimal AmountCommitted { get; set; }
    public int CumulativeParticipants { get; set; }
    public decimal CumulativeAmount { get; set; }
}
