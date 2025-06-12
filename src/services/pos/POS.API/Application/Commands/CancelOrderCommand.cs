namespace eShop.POS.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;

