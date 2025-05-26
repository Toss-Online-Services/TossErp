namespace Ordering.API.Application.Queries;

public class OrderQueries : IOrderQueries
{
    private readonly OrderingContext _orderingContext;
    private readonly ILogger<OrderQueries> _logger;

    public OrderQueries(OrderingContext orderingContext, ILogger<OrderQueries> logger)
    {
        _orderingContext = orderingContext;
        _logger = logger;
    }

    public async Task<OrderViewModel> GetOrderByIdAsync(int id)
    {
        _logger.LogInformation("Getting order {OrderId}", id);

        var order = await _orderingContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return null;
        }

        return new OrderViewModel
        {
            OrderNumber = order.Id,
            Date = order.OrderDate,
            Status = order.OrderStatus.Name,
            Description = order.Description,
            Street = order.Address.Street,
            City = order.Address.City,
            ZipCode = order.Address.ZipCode,
            Country = order.Address.Country,
            OrderItems = order.OrderItems.Select(o => new OrderItemViewModel
            {
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                UnitPrice = o.UnitPrice,
                Units = o.Units,
                PictureUrl = o.PictureUrl
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId)
    {
        _logger.LogInformation("Getting orders for user {UserId}", userId);

        var orders = await _orderingContext.Orders
            .Where(o => o.BuyerId == userId)
            .Select(o => new OrderSummary
            {
                OrderNumber = o.Id,
                Date = o.OrderDate,
                Status = o.OrderStatus.Name,
                Total = o.OrderItems.Sum(i => i.Units * i.UnitPrice)
            })
            .ToListAsync();

        return orders;
    }

    public async Task<IEnumerable<CardType>> GetCardTypesAsync()
    {
        _logger.LogInformation("Getting card types");

        var cardTypes = await _orderingContext.CardTypes
            .ToListAsync();

        return cardTypes;
    }
}
