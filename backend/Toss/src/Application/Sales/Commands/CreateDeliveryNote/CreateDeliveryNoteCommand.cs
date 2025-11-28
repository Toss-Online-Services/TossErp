using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.CreateDeliveryNote;

public record DeliveryNoteLineDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public record CreateDeliveryNoteCommand : IRequest<int>
{
    public int SaleId { get; init; }
    public int ShopId { get; init; }
    public int? PurchaseOrderId { get; init; }
    public string? Notes { get; init; }
    public List<DeliveryNoteLineDto> Lines { get; init; } = new();
}

public class CreateDeliveryNoteCommandHandler : IRequestHandler<CreateDeliveryNoteCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateDeliveryNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateDeliveryNoteCommand request, CancellationToken cancellationToken)
    {
        if (!request.Lines.Any())
        {
            throw new ValidationException("Delivery note requires at least one line.");
        }

        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException(nameof(Sale), request.SaleId.ToString());
        }

        if (sale.ShopId != request.ShopId)
        {
            throw new ValidationException("Sale and delivery note must belong to the same shop.");
        }

        var document = new SalesDocument
        {
            DocumentType = SalesDocumentType.DeliveryNote,
            DocumentNumber = await GenerateDeliveryNumber(request.ShopId, cancellationToken),
            SaleId = sale.Id,
            ShopId = sale.ShopId,
            DocumentDate = DateTimeOffset.UtcNow,
            Notes = request.Notes
        };

        _context.SalesDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        // Update stock levels
        foreach (var line in request.Lines)
        {
            var saleItem = sale.Items.FirstOrDefault(i => i.ProductId == line.ProductId);
            if (saleItem == null)
            {
                throw new ValidationException($"Product {line.ProductId} not found on sale.");
            }

            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == request.ShopId && sl.ProductId == line.ProductId, cancellationToken);

            if (stockLevel == null)
            {
                throw new ValidationException($"Stock not initialized for product {line.ProductId} in shop {request.ShopId}.");
            }

            var before = stockLevel.CurrentStock;
            stockLevel.CurrentStock = Math.Max(0, stockLevel.CurrentStock - line.Quantity);
            stockLevel.LastStockDate = DateTimeOffset.UtcNow;

            var stockMovement = new StockMovement
            {
                ShopId = request.ShopId,
                ProductId = line.ProductId,
                MovementType = StockMovementType.Sale,
                QuantityBefore = before,
                QuantityChange = -line.Quantity,
                QuantityAfter = stockLevel.CurrentStock,
                MovementDate = DateTimeOffset.UtcNow,
                ReferenceType = "DeliveryNote",
                ReferenceId = document.Id,
                Notes = $"Delivery note for sale {sale.SaleNumber}"
            };

            _context.StockMovements.Add(stockMovement);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }

    private async Task<string> GenerateDeliveryNumber(int shopId, CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var prefix = $"DN-{shopId}-{date:yyyyMMdd}-";

        var lastNumber = await _context.SalesDocuments
            .Where(d => d.ShopId == shopId
                        && d.DocumentType == SalesDocumentType.DeliveryNote
                        && d.DocumentDate.Date == date.Date
                        && d.DocumentNumber.StartsWith(prefix))
            .OrderByDescending(d => d.DocumentNumber)
            .Select(d => d.DocumentNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var next = 1;
        if (!string.IsNullOrEmpty(lastNumber))
        {
            var parts = lastNumber.Split('-');
            if (parts.Length >= 4 && int.TryParse(parts[3], out var seq))
            {
                next = seq + 1;
            }
        }

        return $"{prefix}{next:D4}";
    }
}

