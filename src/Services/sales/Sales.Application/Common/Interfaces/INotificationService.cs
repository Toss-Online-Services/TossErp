namespace TossErp.Sales.Application.Common.Interfaces;

/// <summary>
/// Service for sending notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send a notification
    /// </summary>
    /// <param name="recipient">Recipient identifier</param>
    /// <param name="subject">Notification subject</param>
    /// <param name="message">Notification message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendNotificationAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a notification to multiple recipients
    /// </summary>
    /// <param name="recipients">List of recipient identifiers</param>
    /// <param name="subject">Notification subject</param>
    /// <param name="message">Notification message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendNotificationAsync(IEnumerable<string> recipients, string subject, string message, CancellationToken cancellationToken = default);
}
