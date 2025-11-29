using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Queries.GetProductionOrders;

public record GetProductionOrdersQuery : IRequest<PaginatedList<ProductionOrderDto>>
{
    public int? ProductId { get; init; }
    public int? ShopId { get; init; }
    public ProductionOrderStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record ProductionOrderDto
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
    public DateTimeOffset CreatedAt { get; init; }
}

public class GetProductionOrdersQueryHandler : IRequestHandler<GetProductionOrdersQuery, PaginatedList<ProductionOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetProductionOrdersQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<ProductionOrderDto>> Handle(GetProductionOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.ProductionOrders
            .Include(o => o.Product)
            .Include(o => o.Shop)
            .Where(o => o.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(o => o.ProductId == request.ProductId.Value);
        }

        if (request.ShopId.HasValue)
        {
            query = query.Where(o => o.ShopId == request.ShopId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        return await query
            .OrderByDescending(o => o.Created)
            .Select(o => new ProductionOrderDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                ShopId = o.ShopId,
                ShopName = o.Shop.Name,
                PlannedQty = o.PlannedQty,
                Status = o.Status,
                StartedAt = o.StartedAt,
                CompletedAt = o.CompletedAt,
                Notes = o.Notes,
                CreatedAt = o.Created
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

