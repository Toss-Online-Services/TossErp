using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Domain.Events;

/// <summary>
/// Event raised when a supplier price list is created
/// </summary>
public record SupplierPriceListCreatedEvent(Guid PriceListId, Guid SupplierId, string Name, string TenantId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a supplier price list is updated
/// </summary>
public record SupplierPriceListUpdatedEvent(Guid PriceListId, string Name, string? Description) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a supplier price list is activated
/// </summary>
public record SupplierPriceListActivatedEvent(Guid PriceListId, string ActivatedBy) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a supplier price list is deactivated
/// </summary>
public record SupplierPriceListDeactivatedEvent(Guid PriceListId, string DeactivatedBy, string? Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an item is added to a supplier price list
/// </summary>
public record SupplierPriceListItemAddedEvent(Guid PriceListId, Guid ItemId, decimal UnitPrice) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an item price is updated in a supplier price list
/// </summary>
public record SupplierPriceListItemUpdatedEvent(Guid PriceListId, Guid ItemId, decimal NewUnitPrice) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an item is removed from a supplier price list
/// </summary>
public record SupplierPriceListItemRemovedEvent(Guid PriceListId, Guid ItemId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
