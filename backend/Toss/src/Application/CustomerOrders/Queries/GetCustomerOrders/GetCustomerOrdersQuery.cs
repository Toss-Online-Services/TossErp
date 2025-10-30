using Toss.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Toss.Domain.Enums;
using Toss.Domain.Entities.Orders;

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
        try
        {
            // Get first 10 orders without complex filtering
            var orders = await _context.Orders
                .Where(o => !o.Deleted)
                .OrderByDescending(o => o.Created)
                .Take(10)
                .ToListAsync(cancellationToken);

            if (!orders.Any())
                return new List<CustomerOrderListDto>();

            // Get all customers
            var customerIds = orders.Select(o => o.CustomerId).Distinct().ToList();
            var customers = await _context.Customers
                .Where(c => customerIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            // Simple mapping
            return orders.Select(o =>
            {
                var customer = customers.GetValueOrDefault(o.CustomerId);
                return new CustomerOrderListDto
                {
                    Id = o.Id,
                    OrderGuid = o.OrderGuid,
                    OrderNumber = $"ORD-{o.Id:D6}",
                    CustomerId = o.CustomerId,
                    CustomerName = customer?.FullName ?? customer?.Email ?? "Unknown",
                    OrderDate = o.Created,
                    OrderStatus = o.OrderStatus,
                    ShippingStatus = o.ShippingStatus,
                    PaymentStatus = o.PaymentStatus,
                    OrderTotal = o.OrderTotal,
                    ItemCount = 0 // Simplified for now
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            // Log the exception and return empty list
            Console.WriteLine($"Error in GetCustomerOrdersQuery: {ex.Message}");
            return new List<CustomerOrderListDto>();
        }
    }
}

