using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.Common;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void AddDomainEvent(DomainEvent domainEvent);
    void ClearDomainEvents();
} 
