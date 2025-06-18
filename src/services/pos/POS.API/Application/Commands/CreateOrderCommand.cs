using MediatR;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.ValueObjects;
using POS.Domain.SeedWork;
using POS.API.Application.IntegrationEvents;
using POS.API.Application.IntegrationEvents.Events;

namespace POS.API.Application.Commands;

public record CreateOrderCommand : IRequest<Guid>
{
    public string OrderNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
    public decimal TaxRate { get; init; } = 0.1m;
    public decimal DiscountPercentage { get; init; } = 0m;
    public string? Notes { get; init; }
}

public record CreateOrderItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string Currency { get; init; } = "USD";
    public string? Notes { get; init; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IPOSIntegrationEventService _integrationEventService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IRepository<Order> orderRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IPOSIntegrationEventService integrationEventService,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _integrationEventService = integrationEventService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order: {OrderNumber}", request.OrderNumber);

        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        // Create order
        var order = new Order(
            request.OrderNumber,
            request.CustomerId,
            request.TaxRate,
            request.DiscountPercentage,
            request.Notes);

        // Add items to order
        foreach (var itemDto in request.Items)
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {itemDto.ProductId} not found");
            }

            var unitPrice = new Money(itemDto.UnitPrice, itemDto.Currency);
            order.AddItem(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.ProductSku,
                itemDto.Quantity,
                unitPrice,
                itemDto.Notes);
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Publish integration event
        var integrationEvent = new OrderCreatedIntegrationEvent(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.CreatedAt);

        await _integrationEventService.AddAndSaveEventAsync(integrationEvent);
        await _integrationEventService.PublishEventsThroughEventBusAsync(order.Id);

        _logger.LogInformation("Order created successfully with ID: {OrderId}", order.Id);

        return order.Id;
    }
} 
