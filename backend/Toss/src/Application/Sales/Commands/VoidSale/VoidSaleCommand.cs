using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.VoidSale;

public record VoidSaleCommand : IRequest<bool>
{
    public int SaleId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class VoidSaleCommandHandler : IRequestHandler<VoidSaleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public VoidSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(VoidSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new NotFoundException(nameof(Sale), request.SaleId.ToString());

        if (sale.Status == SaleStatus.Voided)
            return false; // Already voided

        // Reverse stock movements
        foreach (var item in sale.Items)
        {
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && sl.ShopId == sale.ShopId, cancellationToken);

            if (stockLevel != null)
            {
                stockLevel.Quantity += item.Quantity; // Add back
            }
        }

        sale.Status = SaleStatus.Voided;
        sale.VoidReason = request.Reason;
        sale.VoidedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

