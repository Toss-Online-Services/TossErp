using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Queries.GetQueueOrders;

public record GetQueueOrdersQuery : IRequest<List<QueueOrderDto>>
{
    public int ShopId { get; init; }
}

public record QueueOrderDto
{
    public int Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public DateTimeOffset SaleDate { get; init; }
    public SaleStatus Status { get; init; }
    public SaleType SaleType { get; init; }
    public decimal Total { get; init; }
    public PaymentType PaymentMethod { get; init; }
    public string? CustomerNotes { get; init; }
    public DateTimeOffset? ExpectedCompletionTime { get; init; }
    public int? QueuePosition { get; init; }
    public List<QueueOrderItemDto> Items { get; init; } = new();
}

public record QueueOrderItemDto
{
    public int Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

public class GetQueueOrdersQueryHandler : IRequestHandler<GetQueueOrdersQuery, List<QueueOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetQueueOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QueueOrderDto>> Handle(GetQueueOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.ShopId == request.ShopId
                && s.SaleType == SaleType.QueueOrder
                && (s.Status == SaleStatus.Pending 
                    || s.Status == SaleStatus.InProgress 
                    || s.Status == SaleStatus.Ready))
            .OrderBy(s => s.QueuePosition ?? int.MaxValue)
            .ThenBy(s => s.SaleDate)
            .Select(s => new QueueOrderDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                ShopId = s.ShopId,
                CustomerId = s.CustomerId,
                CustomerName = s.CustomerName ?? (s.Customer != null ? s.Customer.FirstName + " " + s.Customer.LastName : "Walk-in"),
                CustomerPhone = s.CustomerPhone ?? (s.Customer != null ? s.Customer.PhoneNumber : null),
                SaleDate = s.SaleDate,
                Status = s.Status,
                SaleType = s.SaleType,
                Total = s.Total,
                PaymentMethod = s.PaymentMethod,
                CustomerNotes = s.CustomerNotes,
                ExpectedCompletionTime = s.ExpectedCompletionTime,
                QueuePosition = s.QueuePosition,
                Items = s.Items.Select(i => new QueueOrderItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName ?? "Unknown Product",
                    ProductSKU = i.ProductSKU ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}
