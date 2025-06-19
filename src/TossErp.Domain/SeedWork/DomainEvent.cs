using System;
using MediatR;

namespace TossErp.Domain.SeedWork
{
    public abstract class DomainEvent : INotification
    {
        public Guid Id { get; private set; }
        public DateTime OccurredOn { get; private set; }

        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
} 
