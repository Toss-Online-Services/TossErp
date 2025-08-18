using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Mock implementation of IPaymentGatewayService for MVP
/// </summary>
public class MockPaymentGatewayService : IPaymentGatewayService
{
    private readonly ILogger<MockPaymentGatewayService> _logger;
    private readonly Random _random = new Random();

    public MockPaymentGatewayService(ILogger<MockPaymentGatewayService> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment for sale {SaleId} with amount {Amount} using method {Method}", 
            request.SaleId, request.Amount, request.Method);

        // Simulate processing delay
        await Task.Delay(1000, cancellationToken);

        // Simulate different success rates based on payment method
        var isSuccessful = SimulatePaymentSuccess(request.Method, request.Amount);

        var result = new PaymentResult
        {
            IsSuccessful = isSuccessful,
            PaymentId = GeneratePaymentId(),
            AuthorizationCode = isSuccessful ? GenerateAuthorizationCode() : null,
            TransactionId = isSuccessful ? GenerateTransactionId() : null,
            ErrorMessage = isSuccessful ? null : "Payment failed - insufficient funds",
            ErrorCode = isSuccessful ? null : "INSUFFICIENT_FUNDS",
            ProcessedAt = DateTime.UtcNow,
            Status = isSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed
        };

        _logger.LogInformation("Payment processing result for sale {SaleId}: {Success}", 
            request.SaleId, isSuccessful ? "SUCCESS" : "FAILED");

        return result;
    }

    public async Task<RefundResult> ProcessRefundAsync(RefundRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing refund for payment {PaymentId} with amount {Amount}", 
            request.PaymentId, request.Amount);

        // Simulate processing delay
        await Task.Delay(500, cancellationToken);

        var result = new RefundResult
        {
            IsSuccessful = true, // Refunds are always successful in mock
            RefundId = GenerateRefundId(),
            ProcessedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Refund processing result for payment {PaymentId}: SUCCESS", request.PaymentId);

        return result;
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment status for payment {PaymentId}", paymentId);

        // Simulate processing delay
        await Task.Delay(100, cancellationToken);

        // Mock implementation always returns completed
        return PaymentStatus.Completed;
    }

    private bool SimulatePaymentSuccess(PaymentMethod method, decimal amount)
    {
        // Simulate different success rates
        var successRate = method switch
        {
            PaymentMethod.Cash => 1.0, // Cash always succeeds
            PaymentMethod.Card => 0.95, // 95% success rate for cards
            PaymentMethod.EFT => 0.90, // 90% success rate for EFT
            PaymentMethod.MobileMoney => 0.85, // 85% success rate for mobile money
            _ => 0.80 // 80% success rate for other methods
        };

        // Higher amounts have slightly lower success rates
        if (amount > 1000)
        {
            successRate *= 0.9;
        }

        return _random.NextDouble() < successRate;
    }

    private string GeneratePaymentId()
    {
        return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(100000, 999999)}";
    }

    private string GenerateAuthorizationCode()
    {
        return $"AUTH-{_random.Next(100000, 999999)}";
    }

    private string GenerateTransactionId()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{_random.Next(1000, 9999)}";
    }

    private string GenerateRefundId()
    {
        return $"REF-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(100000, 999999)}";
    }
}
