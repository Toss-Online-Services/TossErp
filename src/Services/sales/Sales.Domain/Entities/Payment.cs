using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Entities;

/// <summary>
/// Payment entity representing a payment made towards a sale
/// </summary>
public class Payment : Entity<Guid>
{
    public PaymentMethod Method { get; private set; }
    public Money Amount { get; private set; } = Money.Zero();
    public string? Reference { get; private set; }
    public string? CardLast4Digits { get; private set; }
    public string? CardType { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    public bool IsSuccessful { get; private set; }
    public string? FailureReason { get; private set; }

    protected Payment() : base() { } // For EF Core

    public Payment(Guid id, PaymentMethod method, Money amount, string? reference = null) : base(id)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));

        Method = method;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Reference = reference;
        ProcessedAt = DateTime.UtcNow;
        IsSuccessful = true; // Assume successful by default for MVP
        TenantId = "default-tenant"; // Will be set by parent Sale
    }

    /// <summary>
    /// Set card payment details
    /// </summary>
    public void SetCardDetails(string last4Digits, string cardType, string authorizationCode)
    {
        if (Method != PaymentMethod.Card)
            throw new InvalidOperationException("Can only set card details for card payments");

        CardLast4Digits = last4Digits;
        CardType = cardType;
        AuthorizationCode = authorizationCode;
    }

    /// <summary>
    /// Mark payment as failed
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        IsSuccessful = false;
        FailureReason = reason;
    }

    /// <summary>
    /// Mark payment as successful
    /// </summary>
    public void MarkAsSuccessful(string? authorizationCode = null)
    {
        IsSuccessful = true;
        FailureReason = null;
        AuthorizationCode = authorizationCode;
    }

    /// <summary>
    /// Set tenant ID for this entity
    /// </summary>
    internal void SetTenantId(string tenantId)
    {
        TenantId = tenantId;
    }
}
