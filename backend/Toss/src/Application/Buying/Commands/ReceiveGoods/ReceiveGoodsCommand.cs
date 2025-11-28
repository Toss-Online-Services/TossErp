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
    public bool QualityCheckPassed { get; init; } = true;
    public string? QualityNotes { get; init; }
}

public record ReceivedItem
{
    public int ProductId { get; init; }
    public int QuantityReceived { get; init; }
}

public class ReceiveGoodsCommandHandler : IRequestHandler<ReceiveGoodsCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public ReceiveGoodsCommandHandler(
        IApplicationDbContext context,
        IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Items)
            .Include(p => p.Shop)
            .FirstOrDefaultAsync(p => p.Id == request.PurchaseOrderId, cancellationToken);

        if (po == null)
            throw new Common.Exceptions.NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

        if (po.Status != PurchaseOrderStatus.Approved && po.Status != PurchaseOrderStatus.PartiallyReceived && po.Status != PurchaseOrderStatus.Confirmed)
            throw new BadRequestException("Only approved, confirmed, or partially received POs can receive goods");

        if (request.Items == null || !request.Items.Any())
            throw new ValidationException("At least one item must be received");

        // Check for idempotency: if a receipt already exists for this PO with the same items, return success
        var existingReceipt = await _context.PurchaseReceipts
            .FirstOrDefaultAsync(r => r.PurchaseOrderId == request.PurchaseOrderId, cancellationToken);
        
        if (existingReceipt != null)
        {
            // For MVP, we'll allow multiple receipts (partial receipts), but we should still process
            // In a production system, you might want stricter idempotency checks
        }

        // Generate GRN (Goods Receipt Note) number
        var receiptNumber = await GenerateReceiptNumber(po.ShopId, cancellationToken);

        // Create PurchaseReceipt (GRN)
        var purchaseReceipt = new PurchaseReceipt
        {
            ReceiptNumber = receiptNumber,
            PurchaseOrderId = po.Id,
            ReceivedDate = DateTimeOffset.UtcNow,
            ReceivedBy = _currentUser.Id ?? "System",
            IsPartialReceipt = false, // Will be determined after processing items
            Notes = request.Notes,
            QualityCheckPassed = request.QualityCheckPassed,
            QualityNotes = request.QualityNotes
        };

        // Process each received item
        var totalItemsReceived = 0;
        var totalItemsOrdered = 0;

        foreach (var receivedItem in request.Items)
        {
            var poItem = po.Items?.FirstOrDefault(i => i.ProductId == receivedItem.ProductId);
            if (poItem == null)
                throw new BadRequestException($"Product {receivedItem.ProductId} not found in PO");

            if (receivedItem.QuantityReceived <= 0)
                throw new ValidationException($"Quantity received must be greater than zero for product {receivedItem.ProductId}");

            // Update PO item received quantity
            poItem.QuantityReceived += receivedItem.QuantityReceived;
            totalItemsReceived += receivedItem.QuantityReceived;
            totalItemsOrdered += poItem.QuantityOrdered;

            // Get or create stock level
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(
                    sl => sl.ShopId == po.ShopId && sl.ProductId == receivedItem.ProductId,
                    cancellationToken);

            var product = await _context.Products.FindAsync(new object[] { receivedItem.ProductId }, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), receivedItem.ProductId);

            var quantityBefore = stockLevel?.CurrentStock ?? 0;
            var quantityChange = receivedItem.QuantityReceived; // Positive for purchases
            var quantityAfter = quantityBefore + quantityChange;

            // Calculate weighted average cost
            // newAvg = (prevQty * prevCost + recvQty * recvCost) / (prevQty + recvQty)
            var previousCost = stockLevel?.AverageCost ?? 0m;
            var receivedCost = poItem.UnitPrice; // Use the PO item's unit price as the received cost
            decimal newAverageCost;

            if (quantityAfter > 0)
            {
                if (quantityBefore > 0)
                {
                    // Weighted average: (prevQty * prevCost + recvQty * recvCost) / (prevQty + recvQty)
                    newAverageCost = ((quantityBefore * previousCost) + (quantityChange * receivedCost)) / quantityAfter;
                }
                else
                {
                    // First receipt: use received cost
                    newAverageCost = receivedCost;
                }
            }
            else
            {
                newAverageCost = previousCost; // No change if quantity is zero
            }

            if (stockLevel == null)
            {
                // Create new stock level if doesn't exist
                stockLevel = new StockLevel
                {
                    ShopId = po.ShopId,
                    ProductId = receivedItem.ProductId,
                    CurrentStock = quantityAfter,
                    AverageCost = newAverageCost,
                    ReorderPoint = product.MinimumStockLevel,
                    ReorderQuantity = product.ReorderQuantity ?? 20,
                    LastStockDate = DateTimeOffset.UtcNow
                };
                _context.StockLevels.Add(stockLevel);
            }
            else
            {
                stockLevel.CurrentStock = quantityAfter;
                stockLevel.AverageCost = newAverageCost;
                stockLevel.LastStockDate = DateTimeOffset.UtcNow;
            }

            // Create stock movement for received goods (ReferenceId will be set after receipt is saved)
            var stockMovement = new StockMovement
            {
                ProductId = receivedItem.ProductId,
                ShopId = po.ShopId,
                QuantityBefore = quantityBefore,
                QuantityChange = quantityChange,
                QuantityAfter = quantityAfter,
                MovementType = StockMovementType.Purchase,
                Notes = $"PO {po.PONumber} - GRN {receiptNumber} - Goods Received",
                ReferenceType = "PurchaseReceipt",
                ReferenceId = null, // Will be set after SaveChanges
                MovementDate = DateTimeOffset.UtcNow
            };

            _context.StockMovements.Add(stockMovement);
        }

        // Determine if fully or partially received
        var allItemsFullyReceived = po.Items?.All(item =>
            item.QuantityReceived >= item.QuantityOrdered
        ) ?? false;

        purchaseReceipt.IsPartialReceipt = !allItemsFullyReceived;

        // Update PO status
        if (allItemsFullyReceived)
        {
            po.Status = PurchaseOrderStatus.Received;
        }
        else if (po.Items?.Any(item => item.QuantityReceived > 0) == true)
        {
            po.Status = PurchaseOrderStatus.PartiallyReceived;
        }

        _context.PurchaseReceipts.Add(purchaseReceipt);

        await _context.SaveChangesAsync(cancellationToken);

        // Update StockMovement ReferenceId now that we have the receipt ID
        var movements = await _context.StockMovements
            .Where(sm => sm.ReferenceType == "PurchaseReceipt" && sm.ReferenceId == null && sm.Notes.Contains(receiptNumber))
            .ToListAsync(cancellationToken);

        foreach (var movement in movements)
        {
            movement.ReferenceId = purchaseReceipt.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<string> GenerateReceiptNumber(int shopId, CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var year = date.Year;
        var existingReceiptNumbers = await _context.PurchaseReceipts
            .Where(r => r.PurchaseOrder.ShopId == shopId && r.ReceivedDate.Year == year)
            .Select(r => r.ReceiptNumber)
            .ToListAsync(cancellationToken);

        var counter = 1;
        if (existingReceiptNumbers.Any())
        {
            var maxCounter = existingReceiptNumbers
                .Where(n => n.StartsWith($"GRN-{year}-"))
                .Select(n => n.Substring($"GRN-{year}-".Length))
                .Where(n => int.TryParse(n, out _))
                .Select(n => int.Parse(n))
                .DefaultIfEmpty(0)
                .Max();
            counter = maxCounter + 1;
        }

        return $"GRN-{year}-{counter:D4}";
    }
}

