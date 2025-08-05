using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record ItemAttributeCreatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeUpdatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeTypeUpdatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeDeactivatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeActivatedEvent(ItemAttribute ItemAttribute) : IDomainEvent;
public record ItemAttributeDeletedEvent(ItemAttribute ItemAttribute) : IDomainEvent; 
