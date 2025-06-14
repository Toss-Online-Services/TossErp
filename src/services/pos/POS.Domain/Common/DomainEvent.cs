using System;

namespace TossErp.POS.Domain.SeedWork;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }

    public bool IsPublished { get; set; }
} 
