using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.Inventory.Commands.CreateProduct;

public record CreateProductCommand : IRequest<int>
{
    public string SKU { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? CategoryId { get; init; }
    public decimal BasePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public string? Unit { get; init; }
    public int MinimumStockLevel { get; init; } = 10;
    public int? ReorderQuantity { get; init; }
    public bool IsTaxable { get; init; } = true;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate SKU
        var existingSKU = await _context.Products
            .AnyAsync(p => p.SKU == request.SKU, cancellationToken);

        if (existingSKU)
            throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists");

        var product = new Product
        {
            SKU = request.SKU,
            Barcode = request.Barcode,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            BasePrice = request.BasePrice,
            CostPrice = request.CostPrice,
            Unit = request.Unit,
            MinimumStockLevel = request.MinimumStockLevel,
            ReorderQuantity = request.ReorderQuantity,
            IsTaxable = request.IsTaxable
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

