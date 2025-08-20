namespace Crm.Application.Commands;

public record RecordPurchaseCommand : IRequest<Unit>
{
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public Guid? OrderId { get; init; }
}

public class RecordPurchaseCommandHandler : IRequestHandler<RecordPurchaseCommand, Unit>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILoyaltyTransactionRepository _loyaltyTransactionRepository;
    private readonly ILogger<RecordPurchaseCommandHandler> _logger;

    public RecordPurchaseCommandHandler(
        ICustomerRepository customerRepository,
        ILoyaltyTransactionRepository loyaltyTransactionRepository,
        ILogger<RecordPurchaseCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _loyaltyTransactionRepository = loyaltyTransactionRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RecordPurchaseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording purchase for customer {CustomerId} with amount {Amount}", 
            request.CustomerId, request.Amount);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        // Record the purchase in the customer entity
        customer.RecordPurchase(request.Amount);

        // Update the customer
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        // Get the latest loyalty transaction that was added
        var loyaltyTransactions = customer.LoyaltyTransactions.ToList();
        var latestTransaction = loyaltyTransactions.LastOrDefault();
        
        if (latestTransaction != null)
        {
            // Update the transaction with the order ID if provided
            if (request.OrderId.HasValue)
            {
                // Note: In a real implementation, you might need to update the transaction
                // This is a simplified version
                _logger.LogInformation("Loyalty points earned: {Points} for order {OrderId}", 
                    latestTransaction.Points, request.OrderId);
            }
        }

        _logger.LogInformation("Purchase recorded successfully for customer {CustomerId}", request.CustomerId);
        return Unit.Value;
    }
}
