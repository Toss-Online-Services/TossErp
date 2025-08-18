namespace TossErp.Procurement.Application.Common.Interfaces;

/// <summary>
/// Service for sending notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send notification to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="type">Notification type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendNotificationAsync(string userId, string title, string message, string type = "info", CancellationToken cancellationToken = default);

    /// <summary>
    /// Send email notification
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send SMS notification
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="message">SMS message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
