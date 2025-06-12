namespace eShop.POS.Infrastructure.Repositories;

public class OrderRepository
    : IOrderRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public OrderRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Order Add(Order order)
    {
        return _context.Orders.Add(order).Entity;

    }

    public async Task<Order> GetAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order != null)
        {
            await _context.Entry(order)
                .Collection(i => i.OrderItems).LoadAsync();
        }

        return order;
    }

    public void Update(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
    }
}
