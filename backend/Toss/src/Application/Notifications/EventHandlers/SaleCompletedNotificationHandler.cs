using Toss.Application.Common.Interfaces.Notifications;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Events;
using Toss.Domain.Enums;
using MediatR;

namespace Toss.Application.Notifications.EventHandlers;

/// <summary>
/// Creates notifications when a sale is completed
/// </summary>
public class SaleCompletedNotificationHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IBusinessContext _businessContext;

    public SaleCompletedNotificationHandler(
        INotificationService notificationService,
        IBusinessContext businessContext)
    {
        _notificationService = notificationService;
        _businessContext = businessContext;
    }

    public Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness) return Task.CompletedTask;

        // Notify business owner/manager about completed sale
        // In a full implementation, you'd get the owner/manager user IDs from the business
        // For MVP, we'll skip this or use a placeholder
        
        // Example: Create success notification for payment received
        // return _notificationService.CreateNotificationAsync(...);
        
        return Task.CompletedTask;
    }
}

