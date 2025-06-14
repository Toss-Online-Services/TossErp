using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : Entity
{
    public int SaleId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Payment() { }

    public Payment(PaymentMethod method, decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        Method = method;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
} 
