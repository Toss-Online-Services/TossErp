namespace Ordering.API.Application.Commands;

public record SetStockRejectedOrderStatusCommand(int OrderNumber, IEnumerable<OrderStockItem> OrderStockItems) : IRequest<bool>;
