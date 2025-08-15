using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TossErp.Stock.Domain.Common;
using eShop.EventBus.Services;

namespace TossErp.Stock.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DispatchDomainEventsInterceptor(
        IMediator mediator,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _mediator = mediator;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        // Publish domain events via MediatR (for internal handlers)
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);

        // Publish integration events via MassTransit (for cross-service communication)
        await _domainEventDispatcher.DispatchDomainEventsAsync(domainEvents);
    }
}
