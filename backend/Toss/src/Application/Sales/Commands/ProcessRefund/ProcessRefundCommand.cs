using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.Sales.Commands.ProcessRefund;

public record ProcessRefundCommand : IRequest<int>
{
    public int SaleId { get; init; }
    public decimal RefundAmount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public bool RestockItems { get; init; } = true;
}

public record RefundResult
{
    public int RefundId { get; init; }
    public decimal RefundAmount { get; init; }
    public bool ItemsRestocked { get; init; }
}

public class ProcessRefundCommandHandler : IRequestHandler<ProcessRefundCommand, int>
{
    private readonly IApplicationDbContext _context;

    public ProcessRefundCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new Common.Exceptions.NotFoundException(nameof(Sale), request.SaleId);

        if (sale.Status != SaleStatus.Completed)
            throw new BadRequestException("Only completed sales can be refunded");

        if (request.RefundAmount > sale.TotalAmount)
            throw new BadRequestException("Refund amount cannot exceed sale total");

        // Update sale status
        sale.Status = SaleStatus.Refunded;
        // LastModified is automatically handled by BaseAuditableEntity interceptor

        // Restock items if requested
        if (request.RestockItems && sale.Items != null)
        {
            foreach (var item in sale.Items)
            {
                var stockLevel = await _context.StockLevels
                    .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && sl.ShopId == sale.ShopId, cancellationToken);

                if (stockLevel != null)
                {
                    // Create stock movement for restock
                    var stockMovement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        ShopId = sale.ShopId,
                        QuantityChange = item.Quantity,
                        MovementType = StockMovementType.Adjustment,
                        Notes = $"Refund - {request.Reason}",
                        ReferenceType = "Refund",
                        ReferenceId = sale.Id,
                        MovementDate = DateTimeOffset.UtcNow
                    };

                    _context.StockMovements.Add(stockMovement);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}

