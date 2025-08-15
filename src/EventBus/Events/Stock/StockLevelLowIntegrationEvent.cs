using System.Text.Json.Serialization;

namespace eShop.EventBus.Events.Stock;

/// <summary>
/// Integration event published when stock level falls below threshold in the Stock service
/// </summary>
public record StockLevelLowIntegrationEvent : IntegrationEvent
{
    public StockLevelLowIntegrationEvent(
        Guid itemId,
        string itemCode,
        string itemName,
        string category,
        Guid warehouseId,
        string warehouseCode,
        string warehouseName,
        Guid? binId,
        string? binCode,
        decimal currentQuantity,
        decimal threshold,
        decimal reorderLevel,
        decimal unitCost,
        decimal totalValue,
        DateTime detectedAt,
        string? detectedBy)
    {
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        Category = category;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        WarehouseName = warehouseName;
        BinId = binId;
        BinCode = binCode;
        CurrentQuantity = currentQuantity;
        Threshold = threshold;
        ReorderLevel = reorderLevel;
        UnitCost = unitCost;
        TotalValue = totalValue;
        DetectedAt = detectedAt;
        DetectedBy = detectedBy;
    }

    [JsonInclude]
    public Guid ItemId { get; init; }

    [JsonInclude]
    public string ItemCode { get; init; } = string.Empty;

    [JsonInclude]
    public string ItemName { get; init; } = string.Empty;

    [JsonInclude]
    public string Category { get; init; } = string.Empty;

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
    public decimal CurrentQuantity { get; init; }

    [JsonInclude]
    public decimal Threshold { get; init; }

    [JsonInclude]
    public decimal ReorderLevel { get; init; }

    [JsonInclude]
    public decimal UnitCost { get; init; }

    [JsonInclude]
    public decimal TotalValue { get; init; }

    [JsonInclude]
    public DateTime DetectedAt { get; init; }

    [JsonInclude]
    public string? DetectedBy { get; init; }
}
