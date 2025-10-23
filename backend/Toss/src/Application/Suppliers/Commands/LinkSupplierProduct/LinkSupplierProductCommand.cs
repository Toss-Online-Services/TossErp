using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Suppliers.Commands.LinkSupplierProduct;

public record LinkSupplierProductCommand : IRequest<int>
{
    public int SupplierId { get; init; }
    public int ProductId { get; init; }
    public string? SupplierSKU { get; init; }
    public decimal BasePrice { get; init; }
    public int LeadTimeDays { get; init; }
    public int MinOrderQuantity { get; init; } = 1;
}

public class LinkSupplierProductCommandHandler : IRequestHandler<LinkSupplierProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public LinkSupplierProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(LinkSupplierProductCommand request, CancellationToken cancellationToken)
    {
        // Check if link already exists
        var existing = await _context.SupplierProducts
            .FirstOrDefaultAsync(sp => sp.SupplierId == request.SupplierId && sp.ProductId == request.ProductId, cancellationToken);

        if (existing != null)
            return existing.Id; // Already linked

        var supplierProduct = new SupplierProduct
        {
            SupplierId = request.SupplierId,
            ProductId = request.ProductId,
            SupplierSKU = request.SupplierSKU,
            BasePrice = request.BasePrice,
            LeadTimeDays = request.LeadTimeDays,
            MinOrderQuantity = request.MinOrderQuantity,
            IsActive = true
        };

        _context.SupplierProducts.Add(supplierProduct);
        await _context.SaveChangesAsync(cancellationToken);

        return supplierProduct.Id;
    }
}

