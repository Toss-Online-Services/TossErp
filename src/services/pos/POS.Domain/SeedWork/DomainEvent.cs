using POS.Domain.Common.Events;
using MediatR;

namespace POS.Domain.SeedWork
{
    public abstract class DomainEvent : INotification
    {
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
} 
