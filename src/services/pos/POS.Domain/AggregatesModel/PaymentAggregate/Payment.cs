using POS.Domain.SeedWork;
using POS.Domain.Models;

namespace POS.Domain.AggregatesModel.PaymentAggregate;

public class Payment : Entity
{
    public string? TransactionId { get; private set; }
    public string? StoreId { get; private set; }
    public string? Status { get; private set; }
    public PaymentMethod? Method { get; private set; }
    public decimal Amount { get; private set; }
    public string? Currency { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Payment() { }

    public Payment(string transactionId, string storeId, PaymentMethod method, decimal amount, string currency)
    {
        TransactionId = transactionId;
        StoreId = storeId;
        Method = method;
        Amount = amount;
        Currency = currency;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }

    public void Process()
    {
        Status = "Processed";
        ProcessedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "Failed";
        ProcessedAt = DateTime.UtcNow;
    }
} 
