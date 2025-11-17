using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Payments.Queries.GetPaymentStatus;

public record PaymentStatusDto
{
    public string TransactionId { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? ResultCode { get; init; }
    public string? ResultDescription { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
}

public record GetPaymentStatusQuery : IRequest<PaymentStatusDto>
{
    public string Provider { get; init; } = string.Empty;
    public string TransactionId { get; init; } = string.Empty;
}

public class GetPaymentStatusQueryHandler : IRequestHandler<GetPaymentStatusQuery, PaymentStatusDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetPaymentStatusQueryHandler> _logger;

    public GetPaymentStatusQueryHandler(
        IApplicationDbContext context,
        ILogger<GetPaymentStatusQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentStatusDto> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        // TODO: Query actual payment status from provider APIs
        // This is a placeholder implementation
        
        _logger.LogInformation("Checking payment status for {Provider} transaction {TransactionId}",
            request.Provider, request.TransactionId);

        // In production, this would:
        // 1. Query M-Pesa/Airtel/MTN API for transaction status
        // 2. Update local database with latest status
        // 3. Return current status

        // For now, return a mock response
        // In reality, we'd query the Payments table for stored transaction
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.TransactionRef == request.TransactionId, cancellationToken);

        if (payment != null)
        {
            return new PaymentStatusDto
            {
                TransactionId = request.TransactionId,
                Provider = request.Provider,
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                ResultCode = "0",
                ResultDescription = payment.Status.ToString(),
                CompletedAt = payment.PaymentDate
            };
        }

        // Mock status for demonstration
        return new PaymentStatusDto
        {
            TransactionId = request.TransactionId,
            Provider = request.Provider,
            Status = "Pending",
            Amount = 0,
            ResultCode = null,
            ResultDescription = "Transaction is being processed"
        };
    }
}

