using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Suppliers.Queries.GetSupplierProducts;

public record SupplierProductDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? SupplierSKU { get; init; }
    public decimal BasePrice { get; init; }
    public int LeadTimeDays { get; init; }
    public int MinOrderQuantity { get; init; }
    public bool IsActive { get; init; }
}

public record GetSupplierProductsQuery : IRequest<List<SupplierProductDto>>
{
    public int SupplierId { get; init; }
    public bool? ActiveOnly { get; init; } = true;
}

public class GetSupplierProductsQueryHandler : IRequestHandler<GetSupplierProductsQuery, List<SupplierProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSupplierProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierProductDto>> Handle(GetSupplierProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SupplierProducts
            .Include(sp => sp.Product)
            .Where(sp => sp.SupplierId == request.SupplierId)
            .AsQueryable();

        if (request.ActiveOnly == true)
        {
            query = query.Where(sp => sp.IsActive);
        }

        var products = await query
            .OrderBy(sp => sp.Product.Name)
            .Select(sp => new SupplierProductDto
            {
                Id = sp.Id,
                ProductId = sp.ProductId,
                ProductName = sp.Product.Name,
                SupplierSKU = sp.SupplierSKU,
                BasePrice = sp.BasePrice,
                LeadTimeDays = sp.LeadTimeDays,
                MinOrderQuantity = sp.MinOrderQuantity,
                IsActive = sp.IsActive
            })
            .ToListAsync(cancellationToken);

        return products;
    }
}

