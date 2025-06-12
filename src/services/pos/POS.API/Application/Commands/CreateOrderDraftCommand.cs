namespace eShop.POS.API.Application.Commands;
using eShop.POS.API.Application.Models;

public record CreateOrderDraftCommand(string BuyerId, IEnumerable<BasketItem> Items) : IRequest<OrderDraftDTO>;
