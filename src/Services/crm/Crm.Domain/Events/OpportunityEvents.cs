using TossErp.CRM.Domain.SeedWork;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Events;

/// <summary>
/// Raised when a new opportunity is created
/// </summary>
public record OpportunityCreatedEvent(
    Guid TenantId,
    Guid OpportunityId,
    Guid CustomerId,
    string OpportunityName,
    OpportunityValue Value,
    string CreatedBy,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an opportunity stage changes
/// </summary>
public record OpportunityStageChangedEvent(
    Guid TenantId,
    Guid OpportunityId,
    OpportunityStage PreviousStage,
    OpportunityStage NewStage,
    decimal NewProbability,
    string ChangedBy,
    decimal PreviousProbability,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an opportunity value is updated
/// </summary>
public record OpportunityValueUpdatedEvent(
    Guid TenantId,
    Guid OpportunityId,
    OpportunityValue PreviousValue,
    OpportunityValue NewValue,
    string UpdatedBy,
    string? Reason = null,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an opportunity is won
/// </summary>
public record OpportunityWonEvent(
    Guid TenantId,
    Guid OpportunityId,
    Guid CustomerId,
    Money ActualValue,
    string WonBy,
    string? WinReason = null,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an opportunity is lost
/// </summary>
public record OpportunityLostEvent(
    Guid TenantId,
    Guid OpportunityId,
    Guid CustomerId,
    string LostBy,
    string LossReason,
    string? CompetitorName = null,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an opportunity is deleted
/// </summary>
public record OpportunityDeletedEvent(
    Guid TenantId,
    Guid OpportunityId,
    Guid CustomerId,
    string DeletedBy,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an activity is scheduled for an opportunity
/// </summary>
public record ActivityScheduledEvent(
    Guid TenantId,
    Guid ActivityId,
    ActivityType ActivityType,
    string Subject,
    DateTime ScheduledAt,
    string CreatedBy,
    string? AssignedTo = null,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

/// <summary>
/// Raised when an activity is completed
/// </summary>
public record ActivityCompletedEvent(
    Guid TenantId,
    Guid ActivityId,
    ActivityType ActivityType,
    string CompletedBy,
    string? Outcome = null,
    string? NextAction = null,
    DateTime OccurredAt = default
) : IDomainEvent
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
