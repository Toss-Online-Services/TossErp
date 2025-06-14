using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : Entity
{
    public int SaleId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Payment()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Payment(int saleId, decimal amount, PaymentMethod paymentMethod)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero");
        if (paymentMethod == null)
            throw new DomainException("Payment amount must be greater than zero");

        Method = method;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
} 
