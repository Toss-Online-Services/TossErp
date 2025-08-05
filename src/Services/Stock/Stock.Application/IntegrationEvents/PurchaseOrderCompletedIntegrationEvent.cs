using eShop.EventBus.Events;

namespace TossErp.Stock.Application.IntegrationEvents;

public record PurchaseOrderCompletedIntegrationEvent(
    string PurchaseOrderNumber,
    string SupplierId,
    DateTime CompletionDate,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<PurchaseOrderItemDto> Items
) : IntegrationEvent;

public record PurchaseOrderItemDto(
    string ItemCode,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    string WarehouseCode,
    decimal QuantityReceived,
    string BatchNumber,
    DateTime? ExpiryDate,
    IReadOnlyList<string> SerialNumbers
);
