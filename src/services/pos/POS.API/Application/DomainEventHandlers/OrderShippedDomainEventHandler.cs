namespace eShop.POS.API.Application.DomainEventHandlers;

public class OrderShippedDomainEventHandler
                : INotificationHandler<OrderShippedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IPOSIntegrationEventService _POSIntegrationEventService;
    private readonly ILogger _logger;

    public OrderShippedDomainEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderShippedDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IPOSIntegrationEventService POSIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _POSIntegrationEventService = POSIntegrationEventService;
    }

    public async Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        POSApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, OrderStatus.Shipped);

        var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToShippedIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await _POSIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
