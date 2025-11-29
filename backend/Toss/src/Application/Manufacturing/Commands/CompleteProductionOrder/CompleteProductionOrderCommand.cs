using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Commands.CompleteProductionOrder;

public record CompleteProductionOrderCommand : IRequest<CompleteProductionOrderResult>
{
    public int ProductionOrderId { get; init; }
    public List<ConsumptionDto>? Consumed { get; init; }
    public List<ProductionDto>? Produced { get; init; }
}

public record ConsumptionDto
{
    public int ComponentProductId { get; init; }
    public decimal Quantity { get; init; }
}

public record ProductionDto
{
    public int Quantity { get; init; }
}

public record CompleteProductionOrderResult
{
    public int ProductionOrderId { get; init; }
    public bool IsCompleted { get; init; }
    public List<string> Warnings { get; init; } = new();
}

public class CompleteProductionOrderCommandHandler : IRequestHandler<CompleteProductionOrderCommand, CompleteProductionOrderResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CompleteProductionOrderCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<CompleteProductionOrderResult> Handle(CompleteProductionOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Get production order with related data
        var order = await _context.ProductionOrders
            .Include(o => o.Product)
            .Include(o => o.Shop)
            .Include(o => o.Consumed)
            .Include(o => o.Produced)
            .FirstOrDefaultAsync(o => o.Id == request.ProductionOrderId
                && o.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException(nameof(ProductionOrder), request.ProductionOrderId.ToString());
        }

        // Validate status
        if (order.Status == ProductionOrderStatus.Completed)
        {
            throw new ValidationException("Production order is already completed.");
        }

        if (order.Status == ProductionOrderStatus.Cancelled)
        {
            throw new ValidationException("Cannot complete a cancelled production order.");
        }

        // Get active BOM
        var bom = await _context.BillOfMaterials
            .Include(b => b.Components)
            .Where(b => b.BusinessId == _businessContext.CurrentBusinessId!.Value
                && b.ProductId == order.ProductId
                && b.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom == null)
        {
            throw new ValidationException($"No active BOM found for product {order.ProductId}.");
        }

        var warnings = new List<string>();
        var consumed = request.Consumed ?? new List<ConsumptionDto>();
        var produced = request.Produced?.FirstOrDefault() ?? new ProductionDto { Quantity = order.PlannedQty };

        // Validate and record consumption
        foreach (var bomComponent in bom.Components)
        {
            var consumptionDto = consumed.FirstOrDefault(c => c.ComponentProductId == bomComponent.ComponentProductId);
            var actualQuantity = consumptionDto?.Quantity ?? (bomComponent.EffectiveQuantity * produced.Quantity);

            if (actualQuantity <= 0)
            {
                throw new ValidationException($"Consumption quantity must be greater than zero for component {bomComponent.ComponentProductId}.");
            }

            // Check stock availability
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == order.ShopId 
                    && sl.ProductId == bomComponent.ComponentProductId, cancellationToken);

            // Business rule: Prevent negative stock
            if (stockLevel == null)
            {
                throw new ValidationException(
                    $"Stock level not found for component {bomComponent.ComponentProductId} at shop {order.ShopId}. " +
                    "Stock level must exist before consumption.");
            }

            if (stockLevel.CurrentStock < actualQuantity)
            {
                throw new ValidationException(
                    $"Insufficient stock for component {bomComponent.ComponentProductId}. " +
                    $"Required: {actualQuantity}, Available: {stockLevel.CurrentStock}. " +
                    "Cannot create negative stock.");
            }

            // Record consumption
            var consumption = new ProductionOrderConsumption
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                ProductionOrderId = order.Id,
                ComponentProductId = bomComponent.ComponentProductId,
                ShopId = order.ShopId,
                Quantity = actualQuantity
            };

            order.Consumed.Add(consumption);

            // Update stock level and create stock movement
            var previousStock = stockLevel.CurrentStock;
            stockLevel.CurrentStock -= (int)Math.Ceiling(actualQuantity);
            stockLevel.LastStockDate = DateTimeOffset.UtcNow;

            var stockMovement = new StockMovement
            {
                ShopId = order.ShopId,
                ProductId = bomComponent.ComponentProductId,
                MovementType = StockMovementType.Consume,
                QuantityBefore = previousStock,
                QuantityChange = -(int)Math.Ceiling(actualQuantity),
                QuantityAfter = stockLevel.CurrentStock,
                ReferenceType = "ProductionOrder",
                ReferenceId = order.Id,
                Notes = $"Production order {order.Id} consumption",
                MovementDate = DateTimeOffset.UtcNow
            };

            _context.StockMovements.Add(stockMovement);
        }

        // Record production
        if (produced.Quantity > 0)
        {
            var production = new ProductionOrderProduction
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                ProductionOrderId = order.Id,
                ProductId = order.ProductId,
                ShopId = order.ShopId,
                Quantity = produced.Quantity
            };

            order.Produced.Add(production);

            // Update finished goods stock level
            var finishedStockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == order.ShopId 
                    && sl.ProductId == order.ProductId, cancellationToken);

            if (finishedStockLevel == null)
            {
                finishedStockLevel = new StockLevel
                {
                    ShopId = order.ShopId,
                    ProductId = order.ProductId,
                    CurrentStock = 0,
                    AverageCost = 0,
                    LastStockDate = DateTimeOffset.UtcNow
                };
                _context.StockLevels.Add(finishedStockLevel);
            }

            var previousFinishedStock = finishedStockLevel.CurrentStock;
            finishedStockLevel.CurrentStock += produced.Quantity;
            finishedStockLevel.LastStockDate = DateTimeOffset.UtcNow;

            // Create stock movement for production
            var productionMovement = new StockMovement
            {
                ShopId = order.ShopId,
                ProductId = order.ProductId,
                MovementType = StockMovementType.Produce,
                QuantityBefore = previousFinishedStock,
                QuantityChange = produced.Quantity,
                QuantityAfter = finishedStockLevel.CurrentStock,
                ReferenceType = "ProductionOrder",
                ReferenceId = order.Id,
                Notes = $"Production order {order.Id} production",
                MovementDate = DateTimeOffset.UtcNow
            };

            _context.StockMovements.Add(productionMovement);
        }

        // Update order status (idempotent - only update if not already completed)
        if (order.Status != ProductionOrderStatus.Completed)
        {
            order.Status = ProductionOrderStatus.Completed;
            order.CompletedAt = DateTimeOffset.UtcNow;
        }

        // All changes are within a transaction - SaveChanges ensures atomicity
        await _context.SaveChangesAsync(cancellationToken);

        return new CompleteProductionOrderResult
        {
            ProductionOrderId = order.Id,
            IsCompleted = true,
            Warnings = warnings
        };
    }
}

