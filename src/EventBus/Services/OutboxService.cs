using System.Text.Json;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus.Services;

/// <summary>
/// Implements the outbox pattern for reliable event publishing
/// </summary>
public class OutboxService : IOutboxService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(IServiceProvider serviceProvider, ILogger<OutboxService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SaveEventAsync(IntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventId = @event.Id,
                EventType = @event.GetType().Name,
                EventData = JsonSerializer.Serialize(@event, @event.GetType()),
                CreatedAt = @event.CreationDate,
                ProcessedAt = null,
                Error = null
            };

            dbContext.Set<OutboxMessage>().Add(outboxMessage);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Saved event {EventType} with ID {EventId} to outbox", @event.GetType().Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving event {EventType} with ID {EventId} to outbox", @event.GetType().Name, @event.Id);
            throw;
        }
    }

    public async Task ProcessOutboxAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var unprocessedMessages = await dbContext.Set<OutboxMessage>()
                .Where(m => m.ProcessedAt == null && m.Error == null)
                .OrderBy(m => m.CreatedAt)
                .Take(100) // Process in batches
                .ToListAsync(cancellationToken);

            foreach (var message in unprocessedMessages)
            {
                try
                {
                    // Deserialize the event
                    var eventType = Type.GetType(message.EventType);
                    if (eventType == null)
                    {
                        _logger.LogWarning("Event type {EventType} not found for message {MessageId}", message.EventType, message.Id);
                        message.Error = $"Event type {message.EventType} not found";
                        continue;
                    }

                    var @event = JsonSerializer.Deserialize(message.EventData, eventType) as IntegrationEvent;
                    if (@event == null)
                    {
                        _logger.LogWarning("Failed to deserialize event for message {MessageId}", message.Id);
                        message.Error = "Failed to deserialize event";
                        continue;
                    }

                    // Publish the event
                    await eventBus.PublishAsync(@event);

                    // Mark as processed
                    message.ProcessedAt = DateTime.UtcNow;
                    _logger.LogInformation("Successfully processed outbox message {MessageId} for event {EventType}", 
                        message.Id, message.EventType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing outbox");
            throw;
        }
    }
}

/// <summary>
/// Outbox message entity for storing events
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Interface for outbox service
/// </summary>
public interface IOutboxService
{
    Task SaveEventAsync(IntegrationEvent @event, CancellationToken cancellationToken = default);
    Task ProcessOutboxAsync(CancellationToken cancellationToken = default);
}
