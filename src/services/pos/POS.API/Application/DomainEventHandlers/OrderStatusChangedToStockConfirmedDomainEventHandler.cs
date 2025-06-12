namespace eShop.POS.API.Application.DomainEventHandlers;

public class OrderStatusChangedToStockConfirmedDomainEventHandler
                : INotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILogger _logger;
    private readonly IPOSIntegrationEventService _POSIntegrationEventService;

    public OrderStatusChangedToStockConfirmedDomainEventHandler(
        IOrderRepository orderRepository,
        IBuyerRepository buyerRepository,
        ILogger<OrderStatusChangedToStockConfirmedDomainEventHandler> logger,
        IPOSIntegrationEventService POSIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _POSIntegrationEventService = POSIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        POSApiTrace.LogOrderStatusUpdated(_logger, domainEvent.OrderId, OrderStatus.StockConfirmed);

        var order = await _orderRepository.GetAsync(domainEvent.OrderId);
        var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await _POSIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
