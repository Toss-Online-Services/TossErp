using eShop.EventBus.Events;

namespace TossErp.Stock.Application.IntegrationEvents;

public record StockLevelUpdatedIntegrationEvent(
    Guid ItemId,
    string ItemCode,
    string ItemName,
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    Guid? BinId,
    string? BinCode,
    decimal PreviousQuantity,
    decimal NewQuantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    decimal UnitCost,
    DateTime UpdatedAt,
    string UpdatedBy
) : IntegrationEvent;
