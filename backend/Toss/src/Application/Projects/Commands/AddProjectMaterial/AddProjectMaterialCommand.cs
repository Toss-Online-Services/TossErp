using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Commands.AddProjectMaterial;

public record AddProjectMaterialCommand : IRequest<int>
{
    public int ProjectId { get; init; }
    public int ProductId { get; init; }
    public int ShopId { get; init; }
    public int Quantity { get; init; }
    public decimal? UnitCost { get; init; }
    public string? Notes { get; init; }
}

public class AddProjectMaterialCommandHandler : IRequestHandler<AddProjectMaterialCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public AddProjectMaterialCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(AddProjectMaterialCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Quantity must be greater than zero.");
        }

        // Validate project exists
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (project == null)
        {
            throw new NotFoundException("Project", request.ProjectId.ToString());
        }

        // Validate product exists
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException("Product", request.ProductId.ToString());
        }

        // Validate shop exists
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.ShopId
                && s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (shop == null)
        {
            throw new NotFoundException("Store", request.ShopId.ToString());
        }

        // Check stock availability
        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ShopId == request.ShopId
                && sl.ProductId == request.ProductId, cancellationToken);

        if (stockLevel == null || stockLevel.CurrentStock < request.Quantity)
        {
            throw new ValidationException(
                $"Insufficient stock for product {product.Name}. " +
                $"Required: {request.Quantity}, Available: {stockLevel?.CurrentStock ?? 0}.");
        }

        // Determine unit cost (use provided, or stock average cost, or product cost price)
        var unitCost = request.UnitCost ?? stockLevel.AverageCost;
        if (unitCost == 0)
        {
            unitCost = product.CostPrice ?? product.BasePrice;
        }

        var totalCost = unitCost * request.Quantity;

        // Create material entry
        var material = new ProjectMaterial
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            ProjectId = request.ProjectId,
            ProductId = request.ProductId,
            ShopId = request.ShopId,
            Quantity = request.Quantity,
            UnitCost = unitCost,
            TotalCost = totalCost,
            Notes = request.Notes
        };

        // Update stock level and create stock movement
        var previousStock = stockLevel.CurrentStock;
        stockLevel.CurrentStock -= request.Quantity;
        stockLevel.LastStockDate = DateTimeOffset.UtcNow;

        var stockMovement = new StockMovement
        {
            ShopId = request.ShopId,
            ProductId = request.ProductId,
            MovementType = StockMovementType.Consume, // Using Consume type for project material consumption
            QuantityBefore = previousStock,
            QuantityChange = -request.Quantity,
            QuantityAfter = stockLevel.CurrentStock,
            ReferenceType = "Project",
            ReferenceId = request.ProjectId,
            Notes = $"Project {project.Title} material consumption",
            MovementDate = DateTimeOffset.UtcNow
        };

        _context.StockMovements.Add(stockMovement);
        material.StockMovementId = stockMovement.Id; // Link after stock movement is saved

        _context.ProjectMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        return material.Id;
    }
}

