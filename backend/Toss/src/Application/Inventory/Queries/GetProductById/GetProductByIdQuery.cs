using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.Inventory.Queries.GetProductById;

public record ProductDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string? Description { get; init; }
    public decimal BasePrice { get; init; }
    public int? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public int TotalStock { get; init; }
    public int MinimumStockLevel { get; init; }
    public bool IsActive { get; init; }
}

public record GetProductByIdQuery : IRequest<ProductDetailDto>
{
    public int Id { get; init; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id.ToString());

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Description = product.Description,
            BasePrice = product.BasePrice,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            TotalStock = product.StockLevels.Sum(sl => sl.AvailableStock),
            MinimumStockLevel = product.MinimumStockLevel,
            IsActive = product.IsActive
        };
    }
}

