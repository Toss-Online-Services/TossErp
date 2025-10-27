using Microsoft.Extensions.Logging;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Payments.Commands.InitiateMtnPayment;

public record MtnPaymentResult
{
    public string TransactionId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? ReferenceId { get; init; }
}

public record InitiateMtnPaymentCommand : IRequest<MtnPaymentResult>
{
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public string? Description { get; init; }
}

public class InitiateMtnPaymentCommandHandler : IRequestHandler<InitiateMtnPaymentCommand, MtnPaymentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<InitiateMtnPaymentCommandHandler> _logger;

    public InitiateMtnPaymentCommandHandler(
        IApplicationDbContext context,
        ILogger<InitiateMtnPaymentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MtnPaymentResult> Handle(InitiateMtnPaymentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with MTN Mobile Money API (MOMO API)
        // This is a placeholder implementation that should be replaced with actual MTN API calls
        
        _logger.LogInformation("Initiating MTN Mobile Money payment for Shop {ShopId}, Amount {Amount}, Phone {Phone}",
            request.ShopId, request.Amount, request.PhoneNumber);

        // Validate shop exists
        var shop = await _context.Shops
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken);

        if (shop == null)
            throw new Toss.Application.Common.Exceptions.NotFoundException("Shop", request.ShopId.ToString());

        // Validate phone number (MTN operates in multiple African countries)
        var validPrefixes = new[] { "233", "234", "237", "242", "250", "256", "260" }; // Ghana, Nigeria, Cameroon, Congo, Rwanda, Uganda, Zambia
        if (!validPrefixes.Any(p => request.PhoneNumber.StartsWith(p)))
        {
            throw new BadRequestException("Phone number must start with valid MTN country code (233, 234, 237, 242, 250, 256, 260)");
        }

        // In production, this would:
        // 1. Get API credentials and generate bearer token
        // 2. Call MTN MOMO Collections API (requestToPay)
        // 3. Store transaction in database
        // 4. Return reference ID for status polling

        // For now, return a mock successful response
        var transactionId = Guid.NewGuid().ToString();

        _logger.LogInformation("MTN Mobile Money payment initiated successfully. TransactionId: {TransactionId}", transactionId);

        return new MtnPaymentResult
        {
            TransactionId = transactionId,
            Status = "Pending",
            Message = "Payment request sent to customer's phone. Awaiting confirmation.",
            ReferenceId = $"MTNPAY{DateTime.UtcNow:yyyyMMddHHmmss}"
        };
    }
}

