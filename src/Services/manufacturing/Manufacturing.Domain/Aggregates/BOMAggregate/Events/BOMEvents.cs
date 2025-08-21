using MediatR;
using TossErp.Manufacturing.Domain.Aggregates.BOMAggregate.Entities;

namespace TossErp.Manufacturing.Domain.Aggregates.BOMAggregate.Events;

// BOM Aggregate Events
public record BOMCreatedEvent(BOMAggregate BOM) : INotification;
public record BOMUpdatedEvent(BOMAggregate BOM) : INotification;
public record BOMSubmittedEvent(BOMAggregate BOM) : INotification;
public record BOMCancelledEvent(BOMAggregate BOM) : INotification;
public record BOMSetAsDefaultEvent(BOMAggregate BOM) : INotification;

// BOM Item Events
public record BOMItemAddedEvent(BOMAggregate BOM, BOMItem Item) : INotification;
public record BOMItemRemovedEvent(BOMAggregate BOM, BOMItem Item) : INotification;
public record BOMItemUpdatedEvent(BOMAggregate BOM, BOMItem Item) : INotification;

// BOM Operation Events
public record BOMOperationAddedEvent(BOMAggregate BOM, BOMOperation Operation) : INotification;
public record BOMOperationRemovedEvent(BOMAggregate BOM, BOMOperation Operation) : INotification;
public record BOMOperationUpdatedEvent(BOMAggregate BOM, BOMOperation Operation) : INotification;
