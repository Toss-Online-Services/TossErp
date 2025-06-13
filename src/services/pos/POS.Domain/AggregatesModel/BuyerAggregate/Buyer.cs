#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : Entity, IAggregateRoot
{
    public string IdentityGuid { get; set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public List<PaymentMethod> PaymentMethods { get; set; } = new();

    protected Buyer() { }

    public Buyer(string identity, string name)
    {
        IdentityGuid = identity;
        Name = name;
    }

    public PaymentMethod VerifyOrAddPaymentMethod(
        CardType cardType, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration,
        int orderId)
    {
        var existingPayment = PaymentMethods
            .SingleOrDefault(p => p.IsEqualTo(cardType.Id, cardNumber, expiration));

        if (existingPayment != null)
        {
            return existingPayment;
        }

        var payment = new PaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration);

        PaymentMethods.Add(payment);

        return payment;
    }
}
