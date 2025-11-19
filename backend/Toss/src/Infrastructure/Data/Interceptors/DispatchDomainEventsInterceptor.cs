using Toss.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;

namespace Toss.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor that automatically dispatches domain events after successful database save operations.
/// Uses a two-phase approach: collects events before save, dispatches after save completes.
/// This ensures database changes are committed before events are published.
/// </summary>
/// <remarks>
/// The interceptor uses a ConcurrentDictionary to handle scenarios where multiple
/// DbContext instances are being used concurrently. Events are cleaned up on failures
/// to prevent stale event dispatches.
/// </remarks>
public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ConcurrentDictionary<DbContext, List<BaseEvent>> _pendingEvents = new();

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Collects domain events before the synchronous save operation.
    /// </summary>
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

    /// <summary>
    /// Collects domain events before the asynchronous save operation.
    /// </summary>
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

    /// <summary>
    /// Dispatches collected domain events after synchronous save completes successfully.
    /// </summary>
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        // Dispatch domain events after SaveChanges completes (IDs are now set)
        if (eventData.Context != null && _pendingEvents.TryRemove(eventData.Context, out var events))
        {
            DispatchDomainEvents(events).GetAwaiter().GetResult();
        }
        
        return base.SavedChanges(eventData, result);
    }

    /// <summary>
    /// Dispatches collected domain events after asynchronous save completes successfully.
    /// </summary>
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        // Dispatch domain events after SaveChanges completes (IDs are now set)
        if (eventData.Context != null && _pendingEvents.TryRemove(eventData.Context, out var events))
        {
            await DispatchDomainEvents(events);
        }
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Cleans up pending events when synchronous save fails to prevent incorrect event dispatch.
    /// </summary>
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        // Clean up pending events if SaveChanges fails (don't dispatch events for failed transactions)
        if (eventData.Context != null)
        {
            _pendingEvents.TryRemove(eventData.Context, out _);
        }
        
        base.SaveChangesFailed(eventData);
    }

    /// <summary>
    /// Cleans up pending events when asynchronous save fails to prevent incorrect event dispatch.
    /// </summary>
    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        // Clean up pending events if SaveChanges fails (don't dispatch events for failed transactions)
        if (eventData.Context != null)
        {
            _pendingEvents.TryRemove(eventData.Context, out _);
        }
        
        await base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    /// <summary>
    /// Collects all domain events from tracked entities and clears them.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>List of collected domain events.</returns>
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

    /// <summary>
    /// Dispatches all collected domain events through MediatR.
    /// </summary>
    /// <param name="domainEvents">List of events to dispatch.</param>
    private async Task DispatchDomainEvents(List<BaseEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }
}
