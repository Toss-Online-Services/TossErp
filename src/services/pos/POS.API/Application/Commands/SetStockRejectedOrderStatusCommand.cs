namespace eShop.POS.API.Application.Commands;

public record SetStockRejectedOrderStatusCommand(int OrderNumber, List<int> OrderStockItems) : IRequest<bool>;
