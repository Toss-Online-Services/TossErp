using eShop.EventBus.Events;

namespace TossErp.Stock.Application.IntegrationEvents;

public record ItemCreatedIntegrationEvent(
    Guid ItemId,
    string ItemCode,
    string ItemName,
    string Description,
    string Category,
    string Unit,
    decimal StandardRate,
    decimal MinimumPrice,
    decimal WeightPerUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy
) : IntegrationEvent;
