using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Payments.Commands.GeneratePaymentQR;

public record PaymentQRResult
{
    public string QRCode { get; init; } = string.Empty;
    public string PaymentUrl { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
}

public record GeneratePaymentQRCommand : IRequest<PaymentQRResult>
{
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string? Description { get; init; }
    public string? CustomerName { get; init; }
}

public class GeneratePaymentQRCommandHandler : IRequestHandler<GeneratePaymentQRCommand, PaymentQRResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GeneratePaymentQRCommandHandler> _logger;

    public GeneratePaymentQRCommandHandler(
        IApplicationDbContext context,
        ILogger<GeneratePaymentQRCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentQRResult> Handle(GeneratePaymentQRCommand request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with QR code generation library (e.g., QRCoder)
        // This is a placeholder implementation
        
        _logger.LogInformation("Generating QR code for Shop {ShopId}, Amount {Amount}",
            request.ShopId, request.Amount);

        // Validate shop exists
        var shop = await _context.Shops
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken);

        if (shop == null)
            throw new NotFoundException("Shop", request.ShopId.ToString());

        // Generate unique payment reference
        var reference = $"QR{DateTime.UtcNow:yyyyMMddHHmmss}{request.ShopId}";

        // In production, this would:
        // 1. Generate QR code image using QRCoder or similar library
        // 2. Encode payment information (amount, shop ID, reference)
        // 3. Store payment link in database
        // 4. Return base64 encoded QR image

        // For now, return a mock QR code (base64 encoded placeholder)
        var mockQRCode = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
            $"TOSS-PAYMENT|SHOP:{request.ShopId}|AMOUNT:{request.Amount}|REF:{reference}"
        ));

        var paymentUrl = $"https://pay.toss.co.za/qr/{reference}";

        _logger.LogInformation("QR code generated successfully. Reference: {Reference}", reference);

        return new PaymentQRResult
        {
            QRCode = mockQRCode,
            PaymentUrl = paymentUrl,
            Reference = reference
        };
    }
}

