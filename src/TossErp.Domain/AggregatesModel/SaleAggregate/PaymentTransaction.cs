using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.SaleAggregate;

public class PaymentTransaction : Entity
{
    public Guid SaleId { get; private set; }
    public decimal Amount { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty; // "pending", "completed", "failed", "refunded"
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    protected PaymentTransaction() 
    {
        PaymentMethod = string.Empty;
        TransactionId = string.Empty;
        Status = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public PaymentTransaction(Guid saleId, decimal amount, string paymentMethod, string transactionId, string status)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        
        if (status == "completed")
        {
            ProcessedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsCompleted()
    {
        Status = "completed";
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = "failed";
        ErrorMessage = errorMessage;
    }

    public void MarkAsRefunded()
    {
        Status = "refunded";
    }
} 
