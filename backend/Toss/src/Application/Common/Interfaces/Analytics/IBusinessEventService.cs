using Toss.Domain.Enums;

namespace Toss.Application.Common.Interfaces.Analytics;

/// <summary>
/// Service for emitting business events for analytics
/// </summary>
public interface IBusinessEventService
{
    /// <summary>
    /// Emits a business event
    /// </summary>
    /// <param name="eventType">The type of event</param>
    /// <param name="module">The module/feature where the event occurred</param>
    /// <param name="eventData">Additional event data as JSON string</param>
    /// <param name="userId">The user ID who triggered the event (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task EmitEventAsync(
        BusinessEventType eventType,
        string? module = null,
        string? eventData = null,
        string? userId = null,
        CancellationToken cancellationToken = default);
}

