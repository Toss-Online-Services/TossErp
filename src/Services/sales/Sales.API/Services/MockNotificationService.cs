using TossErp.Sales.Application.Common.Interfaces;

namespace TossErp.Sales.API.Services;

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

    public Task SendNotificationAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock notification sent to {Recipient}: {Subject} - {Message}", 
            recipient, subject, message);
        return Task.CompletedTask;
    }

    public Task SendNotificationAsync(IEnumerable<string> recipients, string subject, string message, CancellationToken cancellationToken = default)
    {
        var recipientsList = recipients.ToList();
        _logger.LogInformation("Mock notification sent to {RecipientCount} recipients: {Subject} - {Message}", 
            recipientsList.Count, subject, message);
        
        foreach (var recipient in recipientsList)
        {
            _logger.LogInformation("Mock notification sent to {Recipient}: {Subject} - {Message}", 
                recipient, subject, message);
        }
        
        return Task.CompletedTask;
    }
}
