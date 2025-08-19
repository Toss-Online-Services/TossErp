using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Domain.Events;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent : MediatR.INotification
{
    DateTime OccurredOn { get; }
}

/// <summary>
/// Event raised when a cashbook is created
/// </summary>
public record CashbookCreatedEvent(Guid CashbookId, string Name, string TenantId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a cashbook is updated
/// </summary>
public record CashbookUpdatedEvent(Guid CashbookId, string Name, string? Description) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a cashbook is deactivated
/// </summary>
public record CashbookDeactivatedEvent(Guid CashbookId, string DeactivatedBy, string? Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an entry is added to a cashbook
/// </summary>
public record CashbookEntryAddedEvent(Guid CashbookId, Guid EntryId, Money Amount, EntryType Type) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an account is created
/// </summary>
public record AccountCreatedEvent(Guid AccountId, string Code, string Name, AccountType Type, string TenantId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an account is updated
/// </summary>
public record AccountUpdatedEvent(Guid AccountId, string Name, string? Description) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an account is activated
/// </summary>
public record AccountActivatedEvent(Guid AccountId, string ActivatedBy) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an account is deactivated
/// </summary>
public record AccountDeactivatedEvent(Guid AccountId, string DeactivatedBy, string? Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
