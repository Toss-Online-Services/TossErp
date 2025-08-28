using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.Models;

/// <summary>
/// Payment processing request for payment gateway integration
/// </summary>
public class PaymentProcessingRequest
{
    public Guid PaymentId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public PaymentMethod PaymentMethod { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpiryDate { get; set; }
    public string? CVV { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankCode { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? CustomerName { get; set; }
    public string? Reference { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? Description { get; set; }
    public string? CallbackUrl { get; set; }
    public Dictionary<string, string>? AdditionalData { get; set; } = new();
}

/// <summary>
/// Payment processing result from payment gateway
/// </summary>
public class PaymentProcessingResult
{
    public bool IsSuccessful { get; set; }
    public string? TransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public decimal? ProcessingFee { get; set; }
    public string? ProcessorResponse { get; set; }
    public Dictionary<string, string>? AdditionalData { get; set; } = new();

    public static PaymentProcessingResult Success(string transactionId, string? gatewayReference = null)
    {
        return new PaymentProcessingResult
        {
            IsSuccessful = true,
            TransactionId = transactionId,
            GatewayReference = gatewayReference,
            Status = PaymentStatus.Completed,
            ProcessedAt = DateTime.UtcNow
        };
    }

    public static PaymentProcessingResult Failure(string errorMessage, string? errorCode = null)
    {
        return new PaymentProcessingResult
        {
            IsSuccessful = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            Status = PaymentStatus.Failed,
            ProcessedAt = DateTime.UtcNow
        };
    }
}
