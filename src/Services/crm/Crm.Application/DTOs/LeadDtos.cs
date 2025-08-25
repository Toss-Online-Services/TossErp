using TossErp.CRM.Domain.Enums;

namespace Crm.Application.DTOs;

/// <summary>
/// Data Transfer Object for Lead information
/// </summary>
public class LeadDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Company { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Industry { get; set; }
    public int? CompanySize { get; set; }
    public LeadStatus Status { get; set; }
    public LeadSource Source { get; set; }
    public int Score { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastContactedAt { get; set; }
    public DateTime? QualifiedAt { get; set; }
    public bool IsQualified { get; set; }
    public bool IsConverted { get; set; }
    public string? CampaignName { get; set; }
    
    // Address details
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Activity summary
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public DateTime? NextActivityDate { get; set; }
    
    // Conversion tracking
    public DateTime? ConvertedAt { get; set; }
    public Guid? ConvertedCustomerId { get; set; }
    public Guid? ConvertedOpportunityId { get; set; }
}

/// <summary>
/// Simplified Lead DTO for listing views
/// </summary>
public class LeadSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public LeadStatus Status { get; set; }
    public LeadSource Source { get; set; }
    public int Score { get; set; }
    public decimal? EstimatedValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastContactedAt { get; set; }
    public bool IsHot => Score >= 80;
    public bool IsStale => LastContactedAt.HasValue && 
                          (DateTime.UtcNow - LastContactedAt.Value).TotalDays > 30;
}
