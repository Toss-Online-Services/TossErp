using POS.Domain.AggregatesModel.PaymentAggregate.Events;
using POS.Domain.Common;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Models;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : AggregateRoot
{
    public PaymentType Method { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string? Reference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Money AmountObj { get; private set; }
    public string? Notes { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? CardLast4 { get; private set; }
    public string? CardType { get; private set; }
    public Guid SaleId { get; private set; }

    private Payment()
    {
        Method = POS.Domain.Enums.PaymentType.Cash;
        Currency = "USD";
        AmountObj = new Money(0, Currency);
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Payment(Guid id, Guid saleId, decimal amount, POS.Domain.Enums.PaymentType method, string? reference = null, string? cardLast4 = null, string? cardType = null)
    {
        if (id == Guid.Empty)
            throw new DomainException("Payment ID cannot be empty");
        if (saleId == Guid.Empty)
            throw new DomainException("Sale ID cannot be empty");
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero");

        Id = id;
        SaleId = saleId;
        Method = method;
        Currency = "USD";
        Amount = amount;
        AmountObj = new Money(amount, Currency);
        Reference = reference;
        CardLast4 = cardLast4;
        CardType = cardType;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        AddDomainEvent(new PaymentCreatedDomainEvent(Id, saleId, amount, method.ToString()));
    }

    public void AddNote(string note)
    {
        Notes = note;
    }
} 
