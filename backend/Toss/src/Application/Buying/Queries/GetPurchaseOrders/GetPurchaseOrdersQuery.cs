using Toss.Application.Common.Interfaces;

namespace Toss.Application.Buying.Queries.GetPurchaseOrders;

public record PurchaseOrderListDto
{
    public int Id { get; init; }
    public string PONumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public DateTimeOffset OrderDate { get; init; }
    public DateTimeOffset? RequiredDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public record GetPurchaseOrdersQuery : IRequest<List<PurchaseOrderListDto>>
{
    public int? ShopId { get; init; }
    public string? Status { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
}

public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, List<PurchaseOrderListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrderListDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PurchaseOrders
            .Include(p => p.Shop)
            .Include(p => p.Vendor)
            .AsQueryable();

        // Filter by ShopId if provided
        if (request.ShopId.HasValue)
        {
            query = query.Where(p => p.ShopId == request.ShopId.Value);
        }

        // Filter by Status if provided
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(p => p.Status.ToString() == request.Status);
        }

        // Order by OrderDate descending (most recent first)
        query = query.OrderByDescending(p => p.OrderDate);

        // Apply pagination
        var purchaseOrders = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(po => new PurchaseOrderListDto
            {
                Id = po.Id,
                PONumber = po.PONumber,
                ShopId = po.ShopId,
                ShopName = po.Shop.Name,
                VendorId = po.VendorId,
                VendorName = po.Vendor.Name,
                OrderDate = po.OrderDate,
                RequiredDate = po.RequiredDate,
                Status = po.Status.ToString(),
                TotalAmount = po.TotalAmount
            })
            .ToListAsync(cancellationToken);

        return purchaseOrders;
    }
}

