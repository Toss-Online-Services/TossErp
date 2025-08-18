using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Events;

/// <summary>
/// Base class for till domain events
/// </summary>
public abstract record TillDomainEvent : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid TillId { get; init; }

    protected TillDomainEvent(Guid tillId)
    {
        TillId = tillId;
    }
}

/// <summary>
/// Event raised when a till is created
/// </summary>
public record TillCreatedEvent(Guid TillId, string Name, string Code, string TenantId) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is opened
/// </summary>
public record TillOpenedEvent(Guid TillId, Money OpeningBalance, string OpenedBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is closed
/// </summary>
public record TillClosedEvent(Guid TillId, Money FinalBalance, string ClosedBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is suspended
/// </summary>
public record TillSuspendedEvent(Guid TillId, string Reason, string SuspendedBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is resumed
/// </summary>
public record TillResumedEvent(Guid TillId, string ResumedBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when till reconciliation starts
/// </summary>
public record TillReconciliationStartedEvent(Guid TillId, string ReconciledBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is reconciled
/// </summary>
public record TillReconciledEvent(Guid TillId, Money ReconciledBalance, string ReconciledBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is marked out of order
/// </summary>
public record TillMarkedOutOfOrderEvent(Guid TillId, string Reason, string UpdatedBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when a till is restored from out of order
/// </summary>
public record TillRestoredEvent(Guid TillId, string RestoredBy) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when cash is added to a till
/// </summary>
public record TillCashAddedEvent(Guid TillId, Money Amount, string Reason) 
    : TillDomainEvent(TillId);

/// <summary>
/// Event raised when cash is removed from a till
/// </summary>
public record TillCashRemovedEvent(Guid TillId, Money Amount, string Reason) 
    : TillDomainEvent(TillId);
