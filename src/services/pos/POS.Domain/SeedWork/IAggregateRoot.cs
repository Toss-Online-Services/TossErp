namespace POS.Domain.SeedWork
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
} 
