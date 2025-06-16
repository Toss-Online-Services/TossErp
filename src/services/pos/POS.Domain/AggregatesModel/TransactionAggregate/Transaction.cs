using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.TransactionAggregate;

public class Transaction : Entity
{
    public required string Reference { get; set; }
    public required string StoreId { get; set; }
    public required string Status { get; set; }
    public decimal Amount { get; private set; }
    public required string Currency { get; set; }
    public required string PaymentMethod { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? Description { get; private set; }
    public string? Metadata { get; private set; }

    protected Transaction() { }

    public Transaction(string reference, string storeId, decimal amount, string currency, string paymentMethod, string? description = null, string? metadata = null)
    {
        Reference = reference;
        StoreId = storeId;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        Status = "pending";
        CreatedAt = DateTime.UtcNow;
        Description = description;
        Metadata = metadata;
    }

    public void UpdateStatus(string status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAmount(decimal amount)
    {
        Amount = amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMetadata(string metadata)
    {
        Metadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }
} 
