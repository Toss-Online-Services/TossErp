using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Enums;

namespace Toss.Application.Inventory.Commands.AdjustStock;

public record AdjustStockCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int ProductId { get; init; }
    public int QuantityAdjustment { get; init; } // Can be positive or negative
    public StockMovementType MovementType { get; init; }
    public string? Notes { get; init; }
}

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, int>
{
    private readonly IApplicationDbContext _context;

    public AdjustStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ShopId == request.ShopId && sl.ProductId == request.ProductId, cancellationToken);

        if (stockLevel == null)
        {
            // Create new stock level if doesn't exist
            stockLevel = new StockLevel
            {
                ShopId = request.ShopId,
                ProductId = request.ProductId,
                Quantity = Math.Max(0, request.QuantityAdjustment),
                ReorderPoint = 10,
                ReorderQuantity = 20
            };
            _context.StockLevels.Add(stockLevel);
        }
        else
        {
            stockLevel.Quantity += request.QuantityAdjustment;
            if (stockLevel.Quantity < 0)
                stockLevel.Quantity = 0;
        }

        // Record movement
        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            ShopId = request.ShopId,
            Quantity = request.QuantityAdjustment,
            MovementType = request.MovementType,
            MovementDate = DateTime.UtcNow,
            Notes = request.Notes
        };

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync(cancellationToken);

        return movement.Id;
    }
}

