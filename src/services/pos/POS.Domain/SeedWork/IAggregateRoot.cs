using System.Collections.Generic;

namespace TossErp.POS.Domain.SeedWork
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
} 
