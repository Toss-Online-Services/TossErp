using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Enums;

namespace Toss.Application.Inventory.Commands.TransferStock;

public record TransferStockCommand : IRequest<int>
{
    public int FromShopId { get; init; }
    public int ToShopId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
}

public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, int>
{
    private readonly IApplicationDbContext _context;

    public TransferStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(TransferStockCommand request, CancellationToken cancellationToken)
    {
        if (request.FromShopId == request.ToShopId)
        {
            throw new ValidationException("Source and destination shops cannot be the same.");
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Transfer quantity must be greater than zero.");
        }

        // Get source stock level
        var sourceStock = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ShopId == request.FromShopId && sl.ProductId == request.ProductId, cancellationToken);

        if (sourceStock == null || sourceStock.CurrentStock < request.Quantity)
        {
            throw new ValidationException($"Insufficient stock. Available: {sourceStock?.CurrentStock ?? 0}, Requested: {request.Quantity}");
        }

        // Get or create destination stock level
        var destStock = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ShopId == request.ToShopId && sl.ProductId == request.ProductId, cancellationToken);

        if (destStock == null)
        {
            var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.ProductId.ToString());
            }

            destStock = new StockLevel
            {
                ShopId = request.ToShopId,
                ProductId = request.ProductId,
                CurrentStock = 0,
                AverageCost = sourceStock.AverageCost, // Transfer at source cost
                ReorderPoint = product.MinimumStockLevel,
                ReorderQuantity = product.ReorderQuantity ?? 20
            };
            _context.StockLevels.Add(destStock);
        }

        // Update stock levels
        var quantityBeforeSource = sourceStock.CurrentStock;
        sourceStock.CurrentStock -= request.Quantity;
        sourceStock.LastStockDate = DateTimeOffset.UtcNow;

        var quantityBeforeDest = destStock.CurrentStock;
        destStock.CurrentStock += request.Quantity;
        destStock.AverageCost = CalculateWeightedAverage(
            destStock.CurrentStock - request.Quantity,
            destStock.AverageCost,
            request.Quantity,
            sourceStock.AverageCost);
        destStock.LastStockDate = DateTimeOffset.UtcNow;

        // Create outbound movement (source)
        var outboundMovement = new StockMovement
        {
            ShopId = request.FromShopId,
            ProductId = request.ProductId,
            MovementType = StockMovementType.Transfer,
            QuantityBefore = quantityBeforeSource,
            QuantityChange = -request.Quantity,
            QuantityAfter = sourceStock.CurrentStock,
            ReferenceType = "StockTransfer",
            Notes = $"Transfer to Shop {request.ToShopId}. {request.Notes}",
            MovementDate = DateTimeOffset.UtcNow
        };

        // Create inbound movement (destination)
        var inboundMovement = new StockMovement
        {
            ShopId = request.ToShopId,
            ProductId = request.ProductId,
            MovementType = StockMovementType.Transfer,
            QuantityBefore = quantityBeforeDest,
            QuantityChange = request.Quantity,
            QuantityAfter = destStock.CurrentStock,
            ReferenceType = "StockTransfer",
            Notes = $"Transfer from Shop {request.FromShopId}. {request.Notes}",
            MovementDate = DateTimeOffset.UtcNow
        };

        _context.StockMovements.Add(outboundMovement);
        _context.StockMovements.Add(inboundMovement);

        await _context.SaveChangesAsync(cancellationToken);

        return outboundMovement.Id;
    }

    private static decimal CalculateWeightedAverage(int existingQty, decimal existingCost, int newQty, decimal newCost)
    {
        if (existingQty + newQty == 0)
            return 0;

        return ((existingQty * existingCost) + (newQty * newCost)) / (existingQty + newQty);
    }
}

