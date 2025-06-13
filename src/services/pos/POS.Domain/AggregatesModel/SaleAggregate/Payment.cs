#nullable enable
namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : ValueObject
{
    public PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public string? Reference { get; private set; }
    public string? CardLast4 { get; private set; }
    public string? CardType { get; private set; }
    public DateTime PaymentDate { get; private set; }

    protected Payment() { }

    public Payment(PaymentMethod method, decimal amount, string? reference = null, 
        string? cardLast4 = null, string? cardType = null)
    {
        if (amount <= 0)
            throw new POSDomainException("Payment amount must be greater than zero.");

        Method = method;
        Amount = amount;
        Reference = reference ?? string.Empty;
        CardLast4 = cardLast4 ?? string.Empty;
        CardType = cardType ?? string.Empty;
        PaymentDate = DateTime.UtcNow;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Method;
        yield return Amount;
        yield return Reference ?? string.Empty;
        yield return CardLast4 ?? string.Empty;
        yield return CardType ?? string.Empty;
        yield return PaymentDate;
    }
} 
