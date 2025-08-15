using System.Text.Json.Serialization;

namespace eShop.EventBus.Events.Stock;

/// <summary>
/// Integration event published when stock is received in the Stock service
/// </summary>
public record StockReceivedIntegrationEvent : IntegrationEvent
{
    public StockReceivedIntegrationEvent(
        Guid stockMovementId,
        Guid itemId,
        string itemCode,
        string itemName,
        Guid warehouseId,
        string warehouseCode,
        string warehouseName,
        Guid? binId,
        string? binCode,
        decimal quantity,
        decimal unitCost,
        string? batchNo,
        string? serialNo,
        DateTime? expiryDate,
        string movementType,
        string voucherNo,
        string? referenceDocumentType,
        string? referenceDocumentNo,
        DateTime receivedAt,
        string? receivedBy)
    {
        StockMovementId = stockMovementId;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        WarehouseName = warehouseName;
        BinId = binId;
        BinCode = binCode;
        Quantity = quantity;
        UnitCost = unitCost;
        BatchNo = batchNo;
        SerialNo = serialNo;
        ExpiryDate = expiryDate;
        MovementType = movementType;
        VoucherNo = voucherNo;
        ReferenceDocumentType = referenceDocumentType;
        ReferenceDocumentNo = referenceDocumentNo;
        ReceivedAt = receivedAt;
        ReceivedBy = receivedBy;
    }

    [JsonInclude]
    public Guid StockMovementId { get; init; }

    [JsonInclude]
    public Guid ItemId { get; init; }

    [JsonInclude]
    public string ItemCode { get; init; } = string.Empty;

    [JsonInclude]
    public string ItemName { get; init; } = string.Empty;

    [JsonInclude]
    public Guid WarehouseId { get; init; }

    [JsonInclude]
    public string WarehouseCode { get; init; } = string.Empty;

    [JsonInclude]
    public string WarehouseName { get; init; } = string.Empty;

    [JsonInclude]
    public Guid? BinId { get; init; }

    [JsonInclude]
    public string? BinCode { get; init; }

    [JsonInclude]
    public decimal Quantity { get; init; }

    [JsonInclude]
    public decimal UnitCost { get; init; }

    [JsonInclude]
    public string? BatchNo { get; init; }

    [JsonInclude]
    public string? SerialNo { get; init; }

    [JsonInclude]
    public DateTime? ExpiryDate { get; init; }

    [JsonInclude]
    public string MovementType { get; init; } = string.Empty;

    [JsonInclude]
    public string VoucherNo { get; init; } = string.Empty;

    [JsonInclude]
    public string? ReferenceDocumentType { get; init; }

    [JsonInclude]
    public string? ReferenceDocumentNo { get; init; }

    [JsonInclude]
    public DateTime ReceivedAt { get; init; }

    [JsonInclude]
    public string? ReceivedBy { get; init; }
}
