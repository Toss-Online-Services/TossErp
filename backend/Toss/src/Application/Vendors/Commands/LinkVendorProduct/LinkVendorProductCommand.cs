using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Commands.LinkVendorProduct;

public record LinkVendorProductCommand : IRequest<int>
{
    public int VendorId { get; init; }
    public int ProductId { get; init; }
    public string? VendorSKU { get; init; }
    public decimal BasePrice { get; init; }
    public int? LeadTimeDays { get; init; }
    public int MinOrderQuantity { get; init; } = 1;
}

public class LinkVendorProductCommandHandler : IRequestHandler<LinkVendorProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public LinkVendorProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(LinkVendorProductCommand request, CancellationToken cancellationToken)
    {
        // Check if link already exists
        var existing = await _context.VendorProducts
            .FirstOrDefaultAsync(vp => vp.VendorId == request.VendorId && vp.ProductId == request.ProductId, cancellationToken);

        if (existing != null)
            return existing.Id; // Already linked

        var vendorProduct = new VendorProduct
        {
            VendorId = request.VendorId,
            ProductId = request.ProductId,
            VendorSKU = request.VendorSKU,
            BasePrice = request.BasePrice,
            LeadTimeDays = request.LeadTimeDays,
            MinOrderQuantity = request.MinOrderQuantity,
            IsActive = true
        };

        _context.VendorProducts.Add(vendorProduct);
        await _context.SaveChangesAsync(cancellationToken);

        return vendorProduct.Id;
    }
}

