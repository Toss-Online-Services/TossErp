using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Dashboard.Queries.GetOrderStatusDistribution;

public record OrderStatusDistributionDto
{
    public OrderStatus Status { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal Percentage { get; init; }
}

public record GetOrderStatusDistributionQuery : IRequest<List<OrderStatusDistributionDto>>
{
    public int ShopId { get; init; }
}

public class GetOrderStatusDistributionQueryHandler : IRequestHandler<GetOrderStatusDistributionQuery, List<OrderStatusDistributionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderStatusDistributionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderStatusDistributionDto>> Handle(GetOrderStatusDistributionQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Where(o => !o.Deleted)
            .ToListAsync(cancellationToken);

        var totalOrders = orders.Count;

        if (totalOrders == 0)
        {
            return Enum.GetValues<OrderStatus>()
                .Select(status => new OrderStatusDistributionDto
                {
                    Status = status,
                    StatusName = status.ToString(),
                    Count = 0,
                    Percentage = 0
                })
                .ToList();
        }

        var distribution = orders
            .GroupBy(o => o.OrderStatus)
            .Select(g => new OrderStatusDistributionDto
            {
                Status = g.Key,
                StatusName = g.Key.ToString(),
                Count = g.Count(),
                Percentage = (decimal)g.Count() / totalOrders * 100
            })
            .OrderByDescending(d => d.Count)
            .ToList();

        // Ensure all statuses are included
        var allStatuses = Enum.GetValues<OrderStatus>();
        var existingStatuses = distribution.Select(d => d.Status).ToHashSet();
        
        foreach (var status in allStatuses)
        {
            if (!existingStatuses.Contains(status))
            {
                distribution.Add(new OrderStatusDistributionDto
                {
                    Status = status,
                    StatusName = status.ToString(),
                    Count = 0,
                    Percentage = 0
                });
            }
        }

        return distribution.OrderByDescending(d => d.Count).ToList();
    }
}

