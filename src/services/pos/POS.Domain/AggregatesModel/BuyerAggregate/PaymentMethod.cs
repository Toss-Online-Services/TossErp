#nullable enable
using eShop.POS.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace eShop.POS.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    public string Alias { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string SecurityNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public CardType CardType { get; set; } = null!;
    public string BuyerId { get; set; } = string.Empty;

    protected PaymentMethod()
    {
        Alias = string.Empty;
        CardNumber = string.Empty;
        SecurityNumber = string.Empty;
        CardHolderName = string.Empty;
        Expiration = DateTime.MinValue;
        CardType = new CardType { Id = 0, Name = "Unknown" };
    }

    public PaymentMethod(CardType cardType, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        CardType = cardType;
        Alias = alias;
        CardNumber = cardNumber;
        SecurityNumber = securityNumber;
        CardHolderName = cardHolderName;
        Expiration = expiration;
    }

    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return CardType.Id == cardTypeId
            && this.CardNumber == cardNumber
            && this.Expiration == expiration;
    }
}
