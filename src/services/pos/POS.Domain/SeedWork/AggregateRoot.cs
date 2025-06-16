
namespace POS.Domain.SeedWork
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new();

        IReadOnlyCollection<DomainEvent> IAggregateRoot.DomainEvents => throw new NotImplementedException();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
} 
