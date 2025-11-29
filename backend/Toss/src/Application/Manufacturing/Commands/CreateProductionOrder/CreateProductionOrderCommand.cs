using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Manufacturing;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Commands.CreateProductionOrder;

public record CreateProductionOrderCommand : IRequest<int>
{
    public int ProductId { get; init; }
    public int ShopId { get; init; }
    public int PlannedQty { get; init; }
    public string? Notes { get; init; }
    public decimal? OverheadPercent { get; init; }
}

public class CreateProductionOrderCommandHandler : IRequestHandler<CreateProductionOrderCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IManufacturingCostingService _costingService;

    public CreateProductionOrderCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IManufacturingCostingService costingService)
    {
        _context = context;
        _businessContext = businessContext;
        _costingService = costingService;
    }

    public async Task<int> Handle(CreateProductionOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.PlannedQty <= 0)
        {
            throw new ValidationException("Planned quantity must be greater than zero.");
        }

        // Validate product exists and belongs to business
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId.ToString());
        }

        // Validate shop exists and belongs to business
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.ShopId
                && s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (shop == null)
        {
            throw new NotFoundException(nameof(Store), request.ShopId.ToString());
        }

        // Verify active BOM exists
        var bom = await _context.BillOfMaterials
            .Where(b => b.BusinessId == _businessContext.CurrentBusinessId!.Value
                && b.ProductId == request.ProductId
                && b.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom == null)
        {
            throw new ValidationException($"No active BOM found for product {request.ProductId}. Create a BOM first.");
        }

        // Calculate planned cost (for reference, not stored in order)
        var costResult = await _costingService.CalculatePlannedCostAsync(
            request.ProductId,
            request.PlannedQty,
            request.ShopId,
            request.OverheadPercent,
            cancellationToken);

        // Create production order
        var order = new ProductionOrder
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            ProductId = request.ProductId,
            ShopId = request.ShopId,
            PlannedQty = request.PlannedQty,
            Status = ProductionOrderStatus.Draft,
            Notes = request.Notes
        };

        _context.ProductionOrders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

