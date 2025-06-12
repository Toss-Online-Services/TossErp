namespace eShop.POS.API.Application.DomainEventHandlers;

public partial class OrderCancelledDomainEventHandler
                : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILogger _logger;
    private readonly IPOSIntegrationEventService _POSIntegrationEventService;

    public OrderCancelledDomainEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderCancelledDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IPOSIntegrationEventService POSIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _POSIntegrationEventService = POSIntegrationEventService;
    }

    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        POSApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, OrderStatus.Cancelled);

        var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await _POSIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
