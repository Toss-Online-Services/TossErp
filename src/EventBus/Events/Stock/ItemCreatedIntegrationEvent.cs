using System.Text.Json.Serialization;

namespace eShop.EventBus.Events.Stock;

/// <summary>
/// Integration event published when a new item is created in the Stock service
/// </summary>
public record ItemCreatedIntegrationEvent : IntegrationEvent
{
    public ItemCreatedIntegrationEvent(
        Guid itemId,
        string itemCode,
        string itemName,
        string? description,
        string category,
        string unit,
        decimal standardRate,
        decimal minimumPrice,
        decimal? weightPerUnit,
        decimal? length,
        decimal? width,
        decimal? height,
        bool isActive,
        DateTime createdAt,
        string? createdBy)
    {
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        Description = description;
        Category = category;
        Unit = unit;
        StandardRate = standardRate;
        MinimumPrice = minimumPrice;
        WeightPerUnit = weightPerUnit;
        Length = length;
        Width = width;
        Height = height;
        IsActive = isActive;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    [JsonInclude]
    public Guid ItemId { get; init; }

    [JsonInclude]
    public string ItemCode { get; init; } = string.Empty;

    [JsonInclude]
    public string ItemName { get; init; } = string.Empty;

    [JsonInclude]
    public string? Description { get; init; }

    [JsonInclude]
    public string Category { get; init; } = string.Empty;

    [JsonInclude]
    public string Unit { get; init; } = string.Empty;

    [JsonInclude]
    public decimal StandardRate { get; init; }

    [JsonInclude]
    public decimal MinimumPrice { get; init; }

    [JsonInclude]
    public decimal? WeightPerUnit { get; init; }

    [JsonInclude]
    public decimal? Length { get; init; }

    [JsonInclude]
    public decimal? Width { get; init; }

    [JsonInclude]
    public decimal? Height { get; init; }

    [JsonInclude]
    public bool IsActive { get; init; }

    [JsonInclude]
    public DateTime CreatedAt { get; init; }

    [JsonInclude]
    public string? CreatedBy { get; init; }
}
