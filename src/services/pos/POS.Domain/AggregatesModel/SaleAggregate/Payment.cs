using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : AggregateRoot
{
    public Guid SaleId { get; private set; }
    public Sale Sale { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string? Reference { get; private set; }
    public PaymentType Type { get; private set; }
    public string? CardLast4 { get; private set; }
    public string? CardType { get; private set; }
    public string? TransactionId { get; private set; }
    public string Status { get; private set; } = "Pending";
    public string? ErrorMessage { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Payment()
    {
        PaymentDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public Payment(Guid saleId, decimal amount, PaymentType type, string? reference = null, 
        string? cardLast4 = null, string? cardType = null)
    {
        if (saleId == Guid.Empty)
            throw new DomainException("Sale ID cannot be empty");
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        SaleId = saleId;
        Amount = amount;
        Type = type;
        Reference = reference;
        CardLast4 = cardLast4;
        CardType = cardType;
        Status = "Pending";
        PaymentDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateAmount(decimal newAmount)
    {
        if (newAmount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        Amount = newAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed(string? transactionId = null)
    {
        if (Status == "Processed")
            throw new DomainException("Payment is already processed");

        Status = "Processed";
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new DomainException("Error message cannot be empty");

        Status = "Failed";
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        if (Status != "Processed")
            throw new DomainException("Only processed payments can be refunded");

        Status = "Refunded";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsProcessed() => Status == "Processed";
    public bool IsFailed() => Status == "Failed";
    public bool IsRefunded() => Status == "Refunded";
} 
