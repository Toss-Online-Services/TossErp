using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Manufacturing;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Queries.GetBillOfMaterials;

public record GetBillOfMaterialsByIdQuery : IRequest<BillOfMaterialsDetailDto?>
{
    public int Id { get; init; }
}

public record BillOfMaterialsDetailDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string? Version { get; init; }
    public string? Notes { get; init; }
    public List<BomComponentDetailDto> Components { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
}

public record BomComponentDetailDto
{
    public int Id { get; init; }
    public int ComponentProductId { get; init; }
    public string ComponentProductName { get; init; } = string.Empty;
    public decimal QuantityPer { get; init; }
    public string? Unit { get; init; }
    public decimal ScrapPercent { get; init; }
    public decimal EffectiveQuantity { get; init; }
}

public class GetBillOfMaterialsByIdQueryHandler : IRequestHandler<GetBillOfMaterialsByIdQuery, BillOfMaterialsDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetBillOfMaterialsByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<BillOfMaterialsDetailDto?> Handle(GetBillOfMaterialsByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var bom = await _context.BillOfMaterials
            .Include(b => b.Product)
            .Include(b => b.Components)
                .ThenInclude(c => c.ComponentProduct)
            .Where(b => b.Id == request.Id
                && b.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom == null)
        {
            return null;
        }

        return new BillOfMaterialsDetailDto
        {
            Id = bom.Id,
            ProductId = bom.ProductId,
            ProductName = bom.Product.Name,
            IsActive = bom.IsActive,
            Version = bom.Version,
            Notes = bom.Notes,
            Components = bom.Components.Select(c => new BomComponentDetailDto
            {
                Id = c.Id,
                ComponentProductId = c.ComponentProductId,
                ComponentProductName = c.ComponentProduct.Name,
                QuantityPer = c.QuantityPer,
                Unit = c.Unit,
                ScrapPercent = c.ScrapPercent,
                EffectiveQuantity = c.EffectiveQuantity
            }).ToList(),
            CreatedAt = bom.Created
        };
    }
}

