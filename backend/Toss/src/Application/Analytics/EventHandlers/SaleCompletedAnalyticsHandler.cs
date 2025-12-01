using Toss.Application.Common.Interfaces.Analytics;
using Toss.Domain.Events;
using Toss.Domain.Enums;
using System.Text.Json;
using MediatR;

namespace Toss.Application.Analytics.EventHandlers;

/// <summary>
/// Emits analytics event when a sale is completed
/// </summary>
public class SaleCompletedAnalyticsHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IBusinessEventService _eventService;

    public SaleCompletedAnalyticsHandler(IBusinessEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        var sale = notification.Sale;
        var eventData = JsonSerializer.Serialize(new
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.Total,
            PaymentMethod = sale.PaymentMethod.ToString(),
            ItemCount = sale.Items.Count,
            ShopId = sale.ShopId
        });

        await _eventService.EmitEventAsync(
            BusinessEventType.PosSale,
            module: "POS",
            eventData: eventData,
            userId: sale.CreatedBy,
            cancellationToken: cancellationToken);
    }
}

