using TossErp.CRM.Domain.Enums;

namespace Crm.Application.DTOs;

/// <summary>
/// Data Transfer Object for Opportunity information
/// </summary>
public class OpportunityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid? LeadId { get; set; }
    public OpportunityStage Stage { get; set; }
    public OpportunityType Type { get; set; }
    public OpportunityPriority Priority { get; set; }
    public decimal EstimatedValue { get; set; }
    public decimal Probability { get; set; }
    public decimal WeightedValue => EstimatedValue * (Probability / 100);
    public DateTime ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AssignedTo { get; set; }
    public string? SalesTeam { get; set; }
    public LeadSource? Source { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int ContactAttempts { get; set; }
    public DateTime? NextFollowUp { get; set; }
    
    // Closure details
    public string? WinReason { get; set; }
    public string? LossReason { get; set; }
    public string? CompetitorName { get; set; }
    public decimal? ActualValue { get; set; }
    
    // Campaign tracking
    public string? CampaignName { get; set; }
    
    // Status flags
    public bool IsOpen => Stage != OpportunityStage.ClosedWon && Stage != OpportunityStage.ClosedLost;
    public bool IsClosed => Stage == OpportunityStage.ClosedWon || Stage == OpportunityStage.ClosedLost;
    public bool IsWon => Stage == OpportunityStage.ClosedWon;
    public bool IsLost => Stage == OpportunityStage.ClosedLost;
    public bool IsOverdue => IsOpen && ExpectedCloseDate < DateTime.UtcNow.Date;
    public bool IsClosingSoon => IsOpen && ExpectedCloseDate <= DateTime.UtcNow.Date.AddDays(7);
    public int DaysToClose => (ExpectedCloseDate.Date - DateTime.UtcNow.Date).Days;
    
    // Activity summary
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public DateTime? NextActivityDate { get; set; }
}

/// <summary>
/// Simplified Opportunity DTO for pipeline views
/// </summary>
public class OpportunitySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public OpportunityStage Stage { get; set; }
    public decimal EstimatedValue { get; set; }
    public decimal Probability { get; set; }
    public decimal WeightedValue => EstimatedValue * (Probability / 100);
    public DateTime ExpectedCloseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOverdue => ExpectedCloseDate < DateTime.UtcNow.Date;
    public bool IsClosingSoon => ExpectedCloseDate <= DateTime.UtcNow.Date.AddDays(7);
}
