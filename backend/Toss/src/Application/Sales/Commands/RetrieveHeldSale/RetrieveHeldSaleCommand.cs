using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.RetrieveHeldSale;

public record RetrieveHeldSaleCommand : IRequest<bool>
{
    public int SaleId { get; init; }
}

public class RetrieveHeldSaleCommandHandler : IRequestHandler<RetrieveHeldSaleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public RetrieveHeldSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RetrieveHeldSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == request.SaleId && s.Status == SaleStatus.OnHold, cancellationToken);

        if (sale == null)
            return false;

        // Change status from OnHold to Pending so it can be processed
        sale.Status = SaleStatus.Pending;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

