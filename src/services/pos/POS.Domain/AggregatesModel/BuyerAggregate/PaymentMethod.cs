#nullable enable
using eShop.POS.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;
using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    public int BuyerId { get; private set; }
    public string CardType { get; private set; }
    public string Alias { get; private set; }
    public string CardNumber { get; private set; }
    public string SecurityNumber { get; private set; }
    public string CardHolderName { get; private set; }
    public DateTime Expiration { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected PaymentMethod() { }

    public PaymentMethod(int buyerId, string cardType, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        if (string.IsNullOrWhiteSpace(cardType))
            throw new DomainException("Card type cannot be empty");

        if (string.IsNullOrWhiteSpace(alias))
            throw new DomainException("Alias cannot be empty");

        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new DomainException("Card number cannot be empty");

        if (string.IsNullOrWhiteSpace(securityNumber))
            throw new DomainException("Security number cannot be empty");

        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new DomainException("Card holder name cannot be empty");

        if (expiration < DateTime.UtcNow)
            throw new DomainException("Card has expired");

        BuyerId = buyerId;
        CardType = cardType;
        Alias = alias;
        CardNumber = cardNumber;
        SecurityNumber = securityNumber;
        CardHolderName = cardHolderName;
        Expiration = expiration;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return CardType == cardTypeId.ToString()
            && CardNumber == cardNumber
            && Expiration == expiration;
    }
}
