using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Application.Common.Interfaces;

/// <summary>
/// Service for processing payments through payment gateways
/// </summary>
public interface IPaymentGatewayService
{
    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="request">Payment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment result</returns>
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refund a payment
    /// </summary>
    /// <param name="request">Refund request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refund result</returns>
    Task<RefundResult> ProcessRefundAsync(RefundRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payment status
    /// </summary>
    /// <param name="paymentId">Payment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment status</returns>
    Task<PaymentStatus> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Payment request DTO
/// </summary>
public class PaymentRequest
{
    public Guid SaleId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? Reference { get; set; }
    public string? Description { get; set; }
    
    // Card payment details
    public string? CardNumber { get; set; }
    public string? CardExpiryMonth { get; set; }
    public string? CardExpiryYear { get; set; }
    public string? CardCvv { get; set; }
    public string? CardHolderName { get; set; }
    
    // Customer details
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerName { get; set; }
}

/// <summary>
/// Payment result DTO
/// </summary>
public class PaymentResult
{
    public bool IsSuccessful { get; set; }
    public string PaymentId { get; set; } = string.Empty;
    public string? AuthorizationCode { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime ProcessedAt { get; set; }
    public PaymentStatus Status { get; set; }
}

/// <summary>
/// Refund request DTO
/// </summary>
public class RefundRequest
{
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public string? Reference { get; set; }
}

/// <summary>
/// Refund result DTO
/// </summary>
public class RefundResult
{
    public bool IsSuccessful { get; set; }
    public string RefundId { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Payment status enum
/// </summary>
public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Refunded,
    PartiallyRefunded
}
