using Toss.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;

namespace Toss.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ConcurrentDictionary<DbContext, List<BaseEvent>> _pendingEvents = new();

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        // Collect domain events before SaveChanges (entities still in ChangeTracker)
        var domainEvents = CollectDomainEvents(eventData.Context);
        
        // Store events to dispatch after save
        if (domainEvents.Any() && eventData.Context != null)
        {
            _pendingEvents[eventData.Context] = domainEvents;
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        // Collect domain events before SaveChanges (entities still in ChangeTracker)
        var domainEvents = CollectDomainEvents(eventData.Context);
        
        // Store events to dispatch after save
        if (domainEvents.Any() && eventData.Context != null)
        {
            _pendingEvents[eventData.Context] = domainEvents;
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        // Dispatch domain events after SaveChanges completes (IDs are now set)
        if (eventData.Context != null && _pendingEvents.TryRemove(eventData.Context, out var events))
        {
            DispatchDomainEvents(events).GetAwaiter().GetResult();
        }
        
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        // Dispatch domain events after SaveChanges completes (IDs are now set)
        if (eventData.Context != null && _pendingEvents.TryRemove(eventData.Context, out var events))
        {
            await DispatchDomainEvents(events);
        }
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        // Clean up pending events if SaveChanges fails (don't dispatch events for failed transactions)
        if (eventData.Context != null)
        {
            _pendingEvents.TryRemove(eventData.Context, out _);
        }
        
        base.SaveChangesFailed(eventData);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        // Clean up pending events if SaveChanges fails (don't dispatch events for failed transactions)
        if (eventData.Context != null)
        {
            _pendingEvents.TryRemove(eventData.Context, out _);
        }
        
        await base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private List<BaseEvent> CollectDomainEvents(DbContext? context)
    {
        if (context == null) return new List<BaseEvent>();

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events from entities (they'll be dispatched after save)
        entities.ForEach(e => e.ClearDomainEvents());

        return domainEvents;
    }

    private async Task DispatchDomainEvents(List<BaseEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }
}
