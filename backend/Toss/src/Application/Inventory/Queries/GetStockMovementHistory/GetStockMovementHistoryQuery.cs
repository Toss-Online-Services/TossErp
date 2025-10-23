using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Enums;

namespace Toss.Application.Inventory.Queries.GetStockMovementHistory;

public record StockMovementDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string MovementType { get; init; } = string.Empty;
    public DateTime MovementDate { get; init; }
    public string? Notes { get; init; }
}

public record GetStockMovementHistoryQuery : IRequest<PaginatedList<StockMovementDto>>
{
    public int ShopId { get; init; }
    public int? ProductId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetStockMovementHistoryQueryHandler : IRequestHandler<GetStockMovementHistoryQuery, PaginatedList<StockMovementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockMovementHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<StockMovementDto>> Handle(GetStockMovementHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockMovements
            .Include(sm => sm.Product)
            .Where(sm => sm.ShopId == request.ShopId)
            .AsQueryable();

        if (request.ProductId.HasValue)
            query = query.Where(sm => sm.ProductId == request.ProductId.Value);

        if (request.StartDate.HasValue)
            query = query.Where(sm => sm.MovementDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(sm => sm.MovementDate <= request.EndDate.Value);

        var movements = await query
            .OrderByDescending(sm => sm.MovementDate)
            .Select(sm => new StockMovementDto
            {
                Id = sm.Id,
                ProductId = sm.ProductId,
                ProductName = sm.Product.Name,
                Quantity = sm.Quantity,
                MovementType = sm.MovementType.ToString(),
                MovementDate = sm.MovementDate,
                Notes = sm.Notes
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return movements;
    }
}

