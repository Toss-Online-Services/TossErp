namespace TossErp.Stock.Application.DTOs;

/// <summary>
/// Response DTO for creating a new item
/// </summary>
public record CreateItemResponse
{
    /// <summary>
    /// Created item ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Item SKU
    /// </summary>
    public string SKU { get; init; } = string.Empty;

    /// <summary>
    /// Item name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; init; } = "Item created successfully";
}
