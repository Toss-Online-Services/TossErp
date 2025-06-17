using POS.Domain.Common.Events;

namespace POS.Domain.SeedWork
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
} 
