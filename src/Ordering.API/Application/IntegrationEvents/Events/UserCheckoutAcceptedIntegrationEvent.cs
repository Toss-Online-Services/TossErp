namespace Ordering.API.Application.IntegrationEvents.Events;

public record UserCheckoutAcceptedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }
    public string UserName { get; }
    public int CardTypeId { get; }
    public string CardNumber { get; }
    public string CardSecurityNumber { get; }
    public string CardHolderName { get; }
    public DateTime CardExpiration { get; }
    public Address Address { get; }
    public string BuyerIdentityGuid { get; }
    public string RequestId { get; }
    public CustomerBasket Basket { get; }

    public UserCheckoutAcceptedIntegrationEvent(string userId, string userName, int cardTypeId, string cardNumber,
        string cardSecurityNumber, string cardHolderName, DateTime cardExpiration, Address address,
        string buyerIdentityGuid, string requestId, CustomerBasket basket)
    {
        UserId = userId;
        UserName = userName;
        CardTypeId = cardTypeId;
        CardNumber = cardNumber;
        CardSecurityNumber = cardSecurityNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        Address = address;
        BuyerIdentityGuid = buyerIdentityGuid;
        RequestId = requestId;
        Basket = basket;
    }
} 
