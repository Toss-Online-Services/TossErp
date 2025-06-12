namespace eShop.POS.API.Application.Commands;

public record SetAwaitingValidationOrderStatusCommand(int OrderNumber) : IRequest<bool>;
