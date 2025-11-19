using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.DeleteHeldSale;

public record DeleteHeldSaleCommand : IRequest<bool>
{
    public int SaleId { get; init; }
}

public class DeleteHeldSaleCommandHandler : IRequestHandler<DeleteHeldSaleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteHeldSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteHeldSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId && s.Status == SaleStatus.OnHold, cancellationToken);

        if (sale == null)
            return false;

        // Remove the sale and its items
        _context.Sales.Remove(sale);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

