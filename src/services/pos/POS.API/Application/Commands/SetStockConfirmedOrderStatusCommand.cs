namespace eShop.POS.API.Application.Commands;

public record SetStockConfirmedOrderStatusCommand(int OrderNumber) : IRequest<bool>;
