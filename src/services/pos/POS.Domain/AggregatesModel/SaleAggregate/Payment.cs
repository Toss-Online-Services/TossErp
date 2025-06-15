using System;
using TossErp.POS.Domain.Exceptions;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : Entity
{
    public Guid SaleId { get; private set; }
    public decimal Amount { get; private set; }
    public string? Reference { get; private set; }
    public DateTime PaymentDate { get; private set; }

    protected Payment()
    {
    }

    public Payment(Guid saleId, decimal amount, string? reference = null)
    {
        if (saleId == Guid.Empty)
            throw new DomainException("Sale ID cannot be empty");
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        SaleId = saleId;
        Amount = amount;
        Reference = reference;
        PaymentDate = DateTime.UtcNow;
    }

    public void UpdateAmount(decimal newAmount)
    {
        if (newAmount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        Amount = newAmount;
    }
} 
