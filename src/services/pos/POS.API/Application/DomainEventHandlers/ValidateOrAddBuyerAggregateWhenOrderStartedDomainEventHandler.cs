namespace eShop.POS.API.Application.DomainEventHandlers;

public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
                    : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly ILogger _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IPOSIntegrationEventService _POSIntegrationEventService;

    public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
        ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IPOSIntegrationEventService POSIntegrationEventService)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _POSIntegrationEventService = POSIntegrationEventService ?? throw new ArgumentNullException(nameof(POSIntegrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var cardTypeId = domainEvent.CardTypeId != 0 ? domainEvent.CardTypeId : 1;
        var buyer = await _buyerRepository.FindAsync(domainEvent.UserId);
        var buyerExisted = buyer is not null;

        if (!buyerExisted)
        {
            buyer = new Buyer(domainEvent.UserId, domainEvent.UserName);
        }

        // REVIEW: The event this creates needs to be sent after SaveChanges has propagated the buyer Id. It currently only
        // works by coincidence. If we remove HiLo or if anything decides to yield earlier, it will break.

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                        $"Payment Method on {DateTime.UtcNow}",
                                        domainEvent.CardNumber,
                                        domainEvent.CardSecurityNumber,
                                        domainEvent.CardHolderName,
                                        domainEvent.CardExpiration,
                                        domainEvent.Order.Id);

        if (!buyerExisted)
        {
            _buyerRepository.Add(buyer);
        }

        await _buyerRepository.UnitOfWork
            .SaveEntitiesAsync(cancellationToken);

        var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(domainEvent.Order.Id, domainEvent.Order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await _POSIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        POSApiTrace.LogOrderBuyerAndPaymentValidatedOrUpdated(_logger, buyer.Id, domainEvent.Order.Id);
    }
}
