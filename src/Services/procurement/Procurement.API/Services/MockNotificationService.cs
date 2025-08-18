using TossErp.Procurement.Application.Common.Interfaces;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Mock implementation of INotificationService for MVP
/// </summary>
public class MockNotificationService : INotificationService
{
    private readonly ILogger<MockNotificationService> _logger;

    public MockNotificationService(ILogger<MockNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendNotificationAsync(string userId, string title, string message, string type = "info", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock notification sent to user {UserId}: {Title} - {Message} (Type: {Type})", 
            userId, title, message, type);
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock email sent to {Email}: {Subject}", email, subject);
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}
