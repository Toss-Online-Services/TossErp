using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Queries.GetVendorProducts;

public record VendorProductDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? VendorSKU { get; init; }
    public decimal BasePrice { get; init; }
    public int? LeadTimeDays { get; init; }
    public int MinOrderQuantity { get; init; }
    public bool IsActive { get; init; }
}

public record GetVendorProductsQuery : IRequest<List<VendorProductDto>>
{
    public int VendorId { get; init; }
    public bool? ActiveOnly { get; init; } = true;
}

public class GetVendorProductsQueryHandler : IRequestHandler<GetVendorProductsQuery, List<VendorProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVendorProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorProductDto>> Handle(GetVendorProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VendorProducts
            .Include(vp => vp.Product)
            .Where(vp => vp.VendorId == request.VendorId)
            .AsQueryable();

        if (request.ActiveOnly == true)
        {
            query = query.Where(vp => vp.IsActive);
        }

        var products = await query
            .OrderBy(vp => vp.Product.Name)
            .Select(vp => new VendorProductDto
            {
                Id = vp.Id,
                ProductId = vp.ProductId,
                ProductName = vp.Product.Name,
                VendorSKU = vp.VendorSKU,
                BasePrice = vp.BasePrice,
                LeadTimeDays = vp.LeadTimeDays,
                MinOrderQuantity = vp.MinOrderQuantity,
                IsActive = vp.IsActive
            })
            .ToListAsync(cancellationToken);

        return products;
    }
}

