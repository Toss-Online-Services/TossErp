using System.Text.Json.Serialization;

namespace eShop.EventBus.Events.Purchasing;

/// <summary>
/// Integration event published when a purchase order is received in the Purchasing service
/// </summary>
public record PurchaseOrderReceivedIntegrationEvent : IntegrationEvent
{
    public PurchaseOrderReceivedIntegrationEvent(
        Guid purchaseOrderId,
        string purchaseOrderNumber,
        Guid supplierId,
        string supplierName,
        DateTime orderDate,
        DateTime receivedDate,
        decimal totalAmount,
        string currency,
        string status,
        List<PurchaseOrderItemDto> items,
        DateTime receivedAt,
        string? receivedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
        SupplierId = supplierId;
        SupplierName = supplierName;
        OrderDate = orderDate;
        ReceivedDate = receivedDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        Items = items;
        ReceivedAt = receivedAt;
        ReceivedBy = receivedBy;
    }

    [JsonInclude]
    public Guid PurchaseOrderId { get; init; }

    [JsonInclude]
    public string PurchaseOrderNumber { get; init; } = string.Empty;

    [JsonInclude]
    public Guid SupplierId { get; init; }

    [JsonInclude]
    public string SupplierName { get; init; } = string.Empty;

    [JsonInclude]
    public DateTime OrderDate { get; init; }

    [JsonInclude]
    public DateTime ReceivedDate { get; init; }

    [JsonInclude]
    public decimal TotalAmount { get; init; }

    [JsonInclude]
    public string Currency { get; init; } = string.Empty;

    [JsonInclude]
    public string Status { get; init; } = string.Empty;

    [JsonInclude]
    public List<PurchaseOrderItemDto> Items { get; init; } = new();

    [JsonInclude]
    public DateTime ReceivedAt { get; init; }

    [JsonInclude]
    public string? ReceivedBy { get; init; }
}

/// <summary>
/// DTO for purchase order item details
/// </summary>
public record PurchaseOrderItemDto
{
    public PurchaseOrderItemDto(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal orderedQuantity,
        decimal receivedQuantity,
        decimal unitPrice,
        decimal totalPrice,
        string? batchNo,
        DateTime? expiryDate,
        Guid? warehouseId,
        string? warehouseCode,
        Guid? binId,
        string? binCode)
    {
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        OrderedQuantity = orderedQuantity;
        ReceivedQuantity = receivedQuantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        BatchNo = batchNo;
        ExpiryDate = expiryDate;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        BinId = binId;
        BinCode = binCode;
    }

    [JsonInclude]
    public Guid ItemId { get; init; }

    [JsonInclude]
    public string ItemCode { get; init; } = string.Empty;

    [JsonInclude]
    public string ItemName { get; init; } = string.Empty;

    [JsonInclude]
    public decimal OrderedQuantity { get; init; }

    [JsonInclude]
    public decimal ReceivedQuantity { get; init; }

    [JsonInclude]
    public decimal UnitPrice { get; init; }

    [JsonInclude]
    public decimal TotalPrice { get; init; }

    [JsonInclude]
    public string? BatchNo { get; init; }

    [JsonInclude]
    public DateTime? ExpiryDate { get; init; }

    [JsonInclude]
    public Guid? WarehouseId { get; init; }

    [JsonInclude]
    public string? WarehouseCode { get; init; }

    [JsonInclude]
    public Guid? BinId { get; init; }

    [JsonInclude]
    public string? BinCode { get; init; }
}
