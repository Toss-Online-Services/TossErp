using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Manufacturing;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Commands.CreateBillOfMaterials;

public record CreateBillOfMaterialsCommand : IRequest<int>
{
    public int ProductId { get; init; }
    public string? Version { get; init; }
    public string? Notes { get; init; }
    public List<BomComponentDto> Components { get; init; } = new();
}

public record BomComponentDto
{
    public int ComponentProductId { get; init; }
    public decimal QuantityPer { get; init; }
    public string? Unit { get; init; }
    public decimal ScrapPercent { get; init; }
}

public class CreateBillOfMaterialsCommandHandler : IRequestHandler<CreateBillOfMaterialsCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateBillOfMaterialsCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateBillOfMaterialsCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate product exists and belongs to business
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId 
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId.ToString());
        }

        // Deactivate existing active BOM for this product
        var existingBom = await _context.BillOfMaterials
            .Where(b => b.BusinessId == _businessContext.CurrentBusinessId!.Value
                && b.ProductId == request.ProductId
                && b.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingBom != null)
        {
            existingBom.IsActive = false;
        }

        // Create new BOM
        var bom = new BillOfMaterials
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            ProductId = request.ProductId,
            Version = request.Version,
            Notes = request.Notes,
            IsActive = true
        };

        // Validate and add components
        foreach (var componentDto in request.Components)
        {
            if (componentDto.QuantityPer <= 0)
            {
                throw new ValidationException($"Component quantity must be greater than zero for product {componentDto.ComponentProductId}");
            }

            // Validate component product exists and belongs to business
            var componentProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == componentDto.ComponentProductId
                    && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (componentProduct == null)
            {
                throw new NotFoundException(nameof(Product), componentDto.ComponentProductId.ToString());
            }

            // Prevent circular BOM (component cannot be the finished product)
            if (componentDto.ComponentProductId == request.ProductId)
            {
                throw new ValidationException("A product cannot be a component of itself (circular BOM).");
            }

            var component = new BillOfMaterialsComponent
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                BomId = bom.Id, // Will be set after BOM is added
                ComponentProductId = componentDto.ComponentProductId,
                QuantityPer = componentDto.QuantityPer,
                Unit = componentDto.Unit ?? componentProduct.Unit,
                ScrapPercent = componentDto.ScrapPercent
            };

            bom.Components.Add(component);
        }

        if (bom.Components.Count == 0)
        {
            throw new ValidationException("BOM must have at least one component.");
        }

        _context.BillOfMaterials.Add(bom);
        await _context.SaveChangesAsync(cancellationToken);

        return bom.Id;
    }
}

