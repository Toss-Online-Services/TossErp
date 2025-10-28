using Toss.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Toss.Domain.Enums;

namespace Toss.Application.CustomerOrders.Queries.GetCustomerOrders;

public record CustomerOrderListDto
{
    public int Id { get; init; }
    public Guid OrderGuid { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public DateTimeOffset OrderDate { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public ShippingStatus ShippingStatus { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public decimal OrderTotal { get; init; }
    public int ItemCount { get; init; }
}

public record GetCustomerOrdersQuery : IRequest<List<CustomerOrderListDto>>
{
    public int? ShopId { get; init; }
    public int? CustomerId { get; init; }
    public OrderStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, List<CustomerOrderListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerOrderListDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => !o.Deleted);

        if (request.CustomerId.HasValue)
            query = query.Where(o => o.CustomerId == request.CustomerId.Value);

        if (request.Status.HasValue)
            query = query.Where(o => o.OrderStatus == request.Status.Value);

        var orders = await query
            .OrderByDescending(o => o.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new CustomerOrderListDto
            {
                Id = o.Id,
                OrderGuid = o.OrderGuid,
                OrderNumber = $"ORD-{o.Id:D6}",
                CustomerId = o.CustomerId,
                CustomerName = "Customer", // TODO: Join with Customer entity
                OrderDate = o.Created,
                OrderStatus = o.OrderStatus,
                ShippingStatus = o.ShippingStatus,
                PaymentStatus = o.PaymentStatus,
                OrderTotal = o.OrderTotal,
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}

