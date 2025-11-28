using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;

namespace Toss.Application.Stores.Commands.DeleteStore;

public record DeleteStoreCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteStoreCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (store == null)
            throw new Common.Exceptions.NotFoundException(nameof(Store), request.Id.ToString());

        // Check if store has active data
        var hasCustomers = await _context.Customers
            .AnyAsync(c => c.StoreId == request.Id, cancellationToken);
        
        var hasProducts = await _context.StockLevels
            .AnyAsync(sl => sl.ShopId == request.Id, cancellationToken);
        
        var hasSales = await _context.Sales
            .AnyAsync(s => s.ShopId == request.Id, cancellationToken);

        if (hasCustomers || hasProducts || hasSales)
        {
            throw new BadRequestException(
                "Cannot delete store with existing customers, products, or sales. " +
                "Please transfer or remove associated data first.");
        }

        _context.Stores.Remove(store);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

