using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Manufacturing;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Manufacturing;

namespace Toss.Infrastructure.Services.Manufacturing;

public class ManufacturingCostingService : IManufacturingCostingService
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ILogger<ManufacturingCostingService> _logger;
    private const decimal DefaultOverheadPercent = 10.0m; // 10% overhead by default

    public ManufacturingCostingService(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ILogger<ManufacturingCostingService> logger)
    {
        _context = context;
        _businessContext = businessContext;
        _logger = logger;
    }

    public async Task<ManufacturingCostResult> CalculatePlannedCostAsync(
        int productId,
        int plannedQuantity,
        int shopId,
        decimal? overheadPercent = null,
        CancellationToken cancellationToken = default)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new InvalidOperationException("No business context available.");
        }

        // Get active BOM for the product
        var bom = await _context.BillOfMaterials
            .Include(b => b.Components)
                .ThenInclude(c => c.ComponentProduct)
            .Where(b => b.BusinessId == _businessContext.CurrentBusinessId!.Value
                && b.ProductId == productId
                && b.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom == null)
        {
            throw new InvalidOperationException($"No active BOM found for product {productId}");
        }

        if (bom.Components.Count == 0)
        {
            throw new InvalidOperationException($"BOM for product {productId} has no components");
        }

        var overhead = overheadPercent ?? DefaultOverheadPercent;
        var componentCosts = new List<ComponentCostDetail>();
        decimal totalMaterialCost = 0;

        // Calculate cost for each component
        foreach (var component in bom.Components)
        {
            // Get current stock level to determine unit cost (weighted average)
            var stockLevel = await _context.StockLevels
                .Where(sl => sl.ShopId == shopId && sl.ProductId == component.ComponentProductId)
                .FirstOrDefaultAsync(cancellationToken);

            // Use stock level average cost if available, otherwise use product cost price
            decimal unitCost = stockLevel?.AverageCost ?? 0;
            if (unitCost == 0)
            {
                unitCost = component.ComponentProduct.CostPrice ?? component.ComponentProduct.BasePrice;
            }

            // Calculate effective quantity including scrap
            var effectiveQuantity = component.EffectiveQuantity * plannedQuantity;
            var componentTotalCost = effectiveQuantity * unitCost;

            componentCosts.Add(new ComponentCostDetail
            {
                ComponentProductId = component.ComponentProductId,
                ComponentProductName = component.ComponentProduct.Name,
                QuantityPer = component.QuantityPer,
                TotalQuantity = effectiveQuantity,
                UnitCost = unitCost,
                TotalCost = componentTotalCost
            });

            totalMaterialCost += componentTotalCost;
        }

        // Calculate overhead
        var overheadAmount = totalMaterialCost * (overhead / 100);
        var totalCost = totalMaterialCost + overheadAmount;
        var costPerUnit = plannedQuantity > 0 ? totalCost / plannedQuantity : 0;

        _logger.LogInformation(
            "Calculated manufacturing cost for product {ProductId}: Material={MaterialCost}, Overhead={OverheadAmount}, Total={TotalCost}, PerUnit={CostPerUnit}",
            productId,
            totalMaterialCost,
            overheadAmount,
            totalCost,
            costPerUnit);

        return new ManufacturingCostResult
        {
            MaterialCost = totalMaterialCost,
            OverheadAmount = overheadAmount,
            TotalCost = totalCost,
            CostPerUnit = costPerUnit,
            ComponentCosts = componentCosts
        };
    }
}

