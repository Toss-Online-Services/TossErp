using Microsoft.Extensions.Logging;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Payments.Commands.InitiateAirtelPayment;

public record AirtelPaymentResult
{
    public string TransactionId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? ReferenceId { get; init; }
}

public record InitiateAirtelPaymentCommand : IRequest<AirtelPaymentResult>
{
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public string? Description { get; init; }
}

public class InitiateAirtelPaymentCommandHandler : IRequestHandler<InitiateAirtelPaymentCommand, AirtelPaymentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<InitiateAirtelPaymentCommandHandler> _logger;

    public InitiateAirtelPaymentCommandHandler(
        IApplicationDbContext context,
        ILogger<InitiateAirtelPaymentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AirtelPaymentResult> Handle(InitiateAirtelPaymentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with Airtel Money API
        // This is a placeholder implementation that should be replaced with actual Airtel API calls
        
        _logger.LogInformation("Initiating Airtel Money payment for Shop {ShopId}, Amount {Amount}, Phone {Phone}",
            request.ShopId, request.Amount, request.PhoneNumber);

        // Validate shop exists
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken);

        if (shop == null)
            throw new Toss.Application.Common.Exceptions.NotFoundException("Shop", request.ShopId.ToString());

        // Validate phone number (multiple African country codes supported)
        var validPrefixes = new[] { "254", "255", "256", "260", "265", "267" }; // Kenya, Tanzania, Uganda, Zambia, Malawi, Botswana
        if (!validPrefixes.Any(p => request.PhoneNumber.StartsWith(p)))
        {
            throw new BadRequestException("Phone number must start with valid country code (254, 255, 256, 260, 265, 267)");
        }

        // In production, this would:
        // 1. Get OAuth token from Airtel Money API
        // 2. Call Push Payment API
        // 3. Store transaction in database
        // 4. Return reference ID for status polling

        // For now, return a mock successful response
        var transactionId = Guid.NewGuid().ToString();

        _logger.LogInformation("Airtel Money payment initiated successfully. TransactionId: {TransactionId}", transactionId);

        return new AirtelPaymentResult
        {
            TransactionId = transactionId,
            Status = "Pending",
            Message = "Payment request sent to customer's phone. Awaiting confirmation.",
            ReferenceId = $"AIRTELPAY{DateTime.UtcNow:yyyyMMddHHmmss}"
        };
    }
}

