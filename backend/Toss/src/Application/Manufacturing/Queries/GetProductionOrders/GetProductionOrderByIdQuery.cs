using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Queries.GetProductionOrders;

public record GetProductionOrderByIdQuery : IRequest<ProductionOrderDetailDto?>
{
    public int Id { get; init; }
}

public record ProductionOrderDetailDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int PlannedQty { get; init; }
    public ProductionOrderStatus Status { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? Notes { get; init; }
    public List<ConsumptionDetailDto> Consumed { get; init; } = new();
    public List<ProductionDetailDto> Produced { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
}

public record ConsumptionDetailDto
{
    public int Id { get; init; }
    public int ComponentProductId { get; init; }
    public string ComponentProductName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
}

public record ProductionDetailDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

public class GetProductionOrderByIdQueryHandler : IRequestHandler<GetProductionOrderByIdQuery, ProductionOrderDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetProductionOrderByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<ProductionOrderDetailDto?> Handle(GetProductionOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var order = await _context.ProductionOrders
            .Include(o => o.Product)
            .Include(o => o.Shop)
            .Include(o => o.Consumed)
                .ThenInclude(c => c.ComponentProduct)
            .Include(o => o.Produced)
                .ThenInclude(p => p.Product)
            .Where(o => o.Id == request.Id
                && o.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            return null;
        }

        return new ProductionOrderDetailDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            ShopId = order.ShopId,
            ShopName = order.Shop.Name,
            PlannedQty = order.PlannedQty,
            Status = order.Status,
            StartedAt = order.StartedAt,
            CompletedAt = order.CompletedAt,
            Notes = order.Notes,
            Consumed = order.Consumed.Select(c => new ConsumptionDetailDto
            {
                Id = c.Id,
                ComponentProductId = c.ComponentProductId,
                ComponentProductName = c.ComponentProduct.Name,
                Quantity = c.Quantity
            }).ToList(),
            Produced = order.Produced.Select(p => new ProductionDetailDto
            {
                Id = p.Id,
                ProductId = p.ProductId,
                ProductName = p.Product.Name,
                Quantity = p.Quantity
            }).ToList(),
            CreatedAt = order.Created
        };
    }
}

