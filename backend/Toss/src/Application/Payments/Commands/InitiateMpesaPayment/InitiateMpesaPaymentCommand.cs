using Microsoft.Extensions.Logging;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Payments.Commands.InitiateMpesaPayment;

public record MpesaPaymentResult
{
    public string TransactionId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? CheckoutRequestId { get; init; }
}

public record InitiateMpesaPaymentCommand : IRequest<MpesaPaymentResult>
{
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? AccountReference { get; init; }
    public string? TransactionDesc { get; init; }
}

public class InitiateMpesaPaymentCommandHandler : IRequestHandler<InitiateMpesaPaymentCommand, MpesaPaymentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<InitiateMpesaPaymentCommandHandler> _logger;

    public InitiateMpesaPaymentCommandHandler(
        IApplicationDbContext context,
        ILogger<InitiateMpesaPaymentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MpesaPaymentResult> Handle(InitiateMpesaPaymentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with Safaricom M-Pesa Daraja API
        // This is a placeholder implementation that should be replaced with actual M-Pesa API calls
        
        _logger.LogInformation("Initiating M-Pesa payment for Shop {ShopId}, Amount {Amount}, Phone {Phone}",
            request.ShopId, request.Amount, request.PhoneNumber);

        // Validate shop exists
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken);

        if (shop == null)
            throw new Toss.Application.Common.Exceptions.NotFoundException("Shop", request.ShopId.ToString());

        // Validate phone number format (Kenyan format: 254XXXXXXXXX)
        if (!request.PhoneNumber.StartsWith("254") || request.PhoneNumber.Length != 12)
        {
            throw new BadRequestException("Phone number must be in format 254XXXXXXXXX");
        }

        // In production, this would:
        // 1. Get OAuth token from M-Pesa API
        // 2. Call STK Push (Lipa Na M-Pesa Online) API
        // 3. Store transaction in database
        // 4. Return checkout request ID for status polling

        // For now, return a mock successful response
        var transactionId = Guid.NewGuid().ToString();

        _logger.LogInformation("M-Pesa payment initiated successfully. TransactionId: {TransactionId}", transactionId);

        return new MpesaPaymentResult
        {
            TransactionId = transactionId,
            Status = "Pending",
            Message = "Payment request sent to customer's phone. Awaiting confirmation.",
            CheckoutRequestId = $"ws_CO_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..8]}"
        };
    }
}

