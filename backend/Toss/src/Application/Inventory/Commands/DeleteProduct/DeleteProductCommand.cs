using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Inventory.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return false;

        // Soft delete by setting IsActive to false
        // This preserves historical sales data
        product.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

