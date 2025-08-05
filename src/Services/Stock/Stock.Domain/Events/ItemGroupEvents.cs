using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record ItemGroupCreatedEvent(ItemGroup ItemGroup) : IDomainEvent;
public record ItemGroupUpdatedEvent(ItemGroup ItemGroup) : IDomainEvent;
public record ItemGroupDeactivatedEvent(ItemGroup ItemGroup) : IDomainEvent;
public record ItemGroupActivatedEvent(ItemGroup ItemGroup) : IDomainEvent;
public record ItemGroupDeletedEvent(ItemGroup ItemGroup) : IDomainEvent; 
