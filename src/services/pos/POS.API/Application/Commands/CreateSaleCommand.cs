using MediatR;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;
using POS.API.Application.IntegrationEvents;
using POS.API.Application.IntegrationEvents.Events;

namespace POS.API.Application.Commands;

public record CreateSaleCommand : IRequest<Guid>
{
    public string SaleNumber { get; init; } = string.Empty;
    public Guid StoreId { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid? StaffId { get; init; }
    public string? Notes { get; init; }
}

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Store> _storeRepository;
    private readonly IPOSIntegrationEventService _integrationEventService;
    private readonly ILogger<CreateSaleCommandHandler> _logger;

    public CreateSaleCommandHandler(
        IRepository<Sale> saleRepository,
        IRepository<Customer> customerRepository,
        IRepository<Store> storeRepository,
        IPOSIntegrationEventService integrationEventService,
        ILogger<CreateSaleCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _storeRepository = storeRepository;
        _integrationEventService = integrationEventService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating sale: {SaleNumber}", request.SaleNumber);

        // Validate customer exists if provided
        if (request.CustomerId.HasValue)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {request.CustomerId.Value} not found");
            }
        }

        // Validate store exists
        var store = await _storeRepository.GetByIdAsync(request.StoreId);
        if (store == null)
        {
            throw new InvalidOperationException($"Store with ID {request.StoreId} not found");
        }

        var sale = new Sale(
            request.SaleNumber,
            request.StoreId,
            request.CustomerId,
            request.StaffId);

        if (!string.IsNullOrEmpty(request.Notes))
        {
            sale.UpdateNotes(request.Notes);
        }

        await _saleRepository.AddAsync(sale);
        await _saleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Publish integration event
        var integrationEvent = new SaleCompletedIntegrationEvent(
            sale.Id,
            sale.Total,
            sale.CreatedAt);

        await _integrationEventService.AddAndSaveEventAsync(integrationEvent);
        await _integrationEventService.PublishEventsThroughEventBusAsync(sale.Id);

        _logger.LogInformation("Sale created successfully with ID: {SaleId}", sale.Id);

        return sale.Id;
    }
} 
