using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Buying.Commands.ReceiveGoods;

public record ReceiveGoodsCommand : IRequest<bool>
{
    public int PurchaseOrderId { get; init; }
    public List<ReceivedItem> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ReceivedItem
{
    public int ProductId { get; init; }
    public int QuantityReceived { get; init; }
}

public class ReceiveGoodsCommandHandler : IRequestHandler<ReceiveGoodsCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ReceiveGoodsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == request.PurchaseOrderId, cancellationToken);

        if (po == null)
            throw new Common.Exceptions.NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

        if (po.Status != PurchaseOrderStatus.Approved && po.Status != PurchaseOrderStatus.PartiallyReceived)
            throw new BadRequestException("Only approved or partially received POs can receive goods");

        // Process each received item
        foreach (var receivedItem in request.Items)
        {
            var poItem = po.Items?.FirstOrDefault(i => i.ProductId == receivedItem.ProductId);
            if (poItem == null)
                throw new BadRequestException($"Product {receivedItem.ProductId} not found in PO");                                                             

            // Update stock level
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(
                    sl => sl.ShopId == po.ShopId && sl.ProductId == receivedItem.ProductId,
                    cancellationToken);

            var product = await _context.Products.FindAsync(new object[] { receivedItem.ProductId }, cancellationToken);
            var quantityBefore = stockLevel?.Quantity ?? 0;
            var quantityChange = receivedItem.QuantityReceived; // Positive for purchases
            var quantityAfter = quantityBefore + quantityChange;

            if (stockLevel == null)
            {
                // Create new stock level if doesn't exist
                stockLevel = new StockLevel
                {
                    ShopId = po.ShopId,
                    ProductId = receivedItem.ProductId,
                    Quantity = quantityAfter,
                    ReorderPoint = product?.MinimumStockLevel ?? 10,
                    ReorderQuantity = product?.ReorderQuantity ?? 20
                };
                _context.StockLevels.Add(stockLevel);
            }
            else
            {
                stockLevel.Quantity = quantityAfter;
            }

            // Create stock movement for received goods
            var stockMovement = new StockMovement
            {
                ProductId = receivedItem.ProductId,
                ShopId = po.ShopId,
                QuantityBefore = quantityBefore,
                QuantityChange = quantityChange,
                QuantityAfter = quantityAfter,
                MovementType = StockMovementType.Purchase,
                Notes = $"PO {po.PONumber} - Goods Received",
                ReferenceType = "PurchaseOrder",
                ReferenceId = po.Id,
                MovementDate = DateTimeOffset.UtcNow
            };

            _context.StockMovements.Add(stockMovement);
        }

        // Determine if fully or partially received
        var allItemsReceived = po.Items?.All(item =>
            request.Items.Any(ri => ri.ProductId == item.ProductId && ri.QuantityReceived >= item.Quantity)
        ) ?? false;

        po.Status = allItemsReceived ? PurchaseOrderStatus.Received : PurchaseOrderStatus.PartiallyReceived;
        // LastModified is automatically handled by BaseAuditableEntity interceptor
        // Note: PurchaseOrder doesn't have a ReceivedDate property, ApprovedDate is used instead

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

