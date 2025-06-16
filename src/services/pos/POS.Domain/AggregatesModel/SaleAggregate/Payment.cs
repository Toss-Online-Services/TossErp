using POS.Domain.Common.ValueObjects;
using POS.Domain.Models;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : Entity
{
    public POS.Domain.Models.PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string? Reference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Money AmountObj { get; private set; }
    public string? Notes { get; private set; }

    private Payment() { } // For EF Core

    public Payment(POS.Domain.Models.PaymentMethod method, decimal amount, string currency, string? reference = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        Method = method;
        Amount = amount;
        Currency = currency;
        Reference = reference;
        CreatedAt = DateTime.UtcNow;
        AmountObj = new Money(amount, currency);
    }

    public void AddNote(string note)
    {
        Notes = note;
    }
} 
