using System.Text.Json.Serialization;

namespace eShop.EventBus.Events.Sales;

/// <summary>
/// Integration event published when a sale is completed in the Sales service
/// </summary>
public record SaleCompletedIntegrationEvent : IntegrationEvent
{
    public SaleCompletedIntegrationEvent(
        Guid saleId,
        string saleNumber,
        Guid customerId,
        string customerName,
        DateTime saleDate,
        decimal totalAmount,
        string currency,
        string status,
        List<SaleItemDto> items,
        DateTime completedAt,
        string? completedBy)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        SaleDate = saleDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        Items = items;
        CompletedAt = completedAt;
        CompletedBy = completedBy;
    }

    [JsonInclude]
    public Guid SaleId { get; init; }

    [JsonInclude]
    public string SaleNumber { get; init; } = string.Empty;

    [JsonInclude]
    public Guid CustomerId { get; init; }

    [JsonInclude]
    public string CustomerName { get; init; } = string.Empty;

    [JsonInclude]
    public DateTime SaleDate { get; init; }

    [JsonInclude]
    public decimal TotalAmount { get; init; }

    [JsonInclude]
    public string Currency { get; init; } = string.Empty;

    [JsonInclude]
    public string Status { get; init; } = string.Empty;

    [JsonInclude]
    public List<SaleItemDto> Items { get; init; } = new();

    [JsonInclude]
    public DateTime CompletedAt { get; init; }

    [JsonInclude]
    public string? CompletedBy { get; init; }
}

/// <summary>
/// DTO for sale item details
/// </summary>
public record SaleItemDto
{
    public SaleItemDto(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        Guid? warehouseId,
        string? warehouseCode,
        Guid? binId,
        string? binCode)
    {
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
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
    public decimal Quantity { get; init; }

    [JsonInclude]
    public decimal UnitPrice { get; init; }

    [JsonInclude]
    public decimal TotalPrice { get; init; }

    [JsonInclude]
    public Guid? WarehouseId { get; init; }

    [JsonInclude]
    public string? WarehouseCode { get; init; }

    [JsonInclude]
    public Guid? BinId { get; init; }

    [JsonInclude]
    public string? BinCode { get; init; }
}
