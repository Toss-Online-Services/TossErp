namespace Toss.Application.Common.Interfaces.Manufacturing;

/// <summary>
/// Service for calculating manufacturing costs including BOM material costs and overhead
/// </summary>
public interface IManufacturingCostingService
{
    /// <summary>
    /// Calculates the planned cost for a production order based on BOM and current product costs
    /// </summary>
    /// <param name="productId">The finished product ID</param>
    /// <param name="plannedQuantity">The planned quantity to produce</param>
    /// <param name="shopId">The shop where production occurs</param>
    /// <param name="overheadPercent">Optional overhead percentage (default from config)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total planned cost including materials and overhead</returns>
    Task<ManufacturingCostResult> CalculatePlannedCostAsync(
        int productId,
        int plannedQuantity,
        int shopId,
        decimal? overheadPercent = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of manufacturing cost calculation
/// </summary>
public record ManufacturingCostResult
{
    public decimal MaterialCost { get; init; }
    public decimal OverheadAmount { get; init; }
    public decimal TotalCost { get; init; }
    public decimal CostPerUnit { get; init; }
    public List<ComponentCostDetail> ComponentCosts { get; init; } = new();
}

/// <summary>
/// Cost detail for a BOM component
/// </summary>
public record ComponentCostDetail
{
    public int ComponentProductId { get; init; }
    public string ComponentProductName { get; init; } = string.Empty;
    public decimal QuantityPer { get; init; }
    public decimal TotalQuantity { get; init; }
    public decimal UnitCost { get; init; }
    public decimal TotalCost { get; init; }
}

