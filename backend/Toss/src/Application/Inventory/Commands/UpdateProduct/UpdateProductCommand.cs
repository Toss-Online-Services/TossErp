using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Inventory.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<bool>
{
    public int Id { get; init; }
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
    public bool IsActive { get; init; } = true;
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return false;

        // Check for duplicate SKU (excluding current product)
        var existingSKU = await _context.Products
            .AnyAsync(p => p.SKU == request.SKU && p.Id != request.Id, cancellationToken);

        if (existingSKU)
            throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists");

        product.SKU = request.SKU;
        product.Barcode = request.Barcode;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.BasePrice = request.BasePrice;
        product.CostPrice = request.CostPrice;
        product.Unit = request.Unit;
        product.MinimumStockLevel = request.MinimumStockLevel;
        product.ReorderQuantity = request.ReorderQuantity;
        product.IsTaxable = request.IsTaxable;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

