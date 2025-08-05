using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record ItemAlternativeCreatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeUpdatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativePreferredSetEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeDeactivatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeActivatedEvent(ItemAlternative ItemAlternative) : IDomainEvent;
public record ItemAlternativeDeletedEvent(ItemAlternative ItemAlternative) : IDomainEvent; 
