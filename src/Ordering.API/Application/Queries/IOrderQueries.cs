namespace Ordering.API.Application.Queries;

public interface IOrderQueries
{
    Task<OrderViewModel> GetOrderByIdAsync(int id);

    Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId);

    Task<IEnumerable<CardType>> GetCardTypesAsync();
}
