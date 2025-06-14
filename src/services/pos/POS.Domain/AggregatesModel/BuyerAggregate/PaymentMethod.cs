#nullable enable
using eShop.POS.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;
using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    public int BuyerId { get; private set; }
    public string CardNumber { get; private set; }
    public string CardHolderName { get; private set; }
    public string ExpirationDate { get; private set; }
    public string SecurityCode { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private PaymentMethod() { }

    public PaymentMethod(string cardNumber, string cardHolderName, string expirationDate, string securityCode, bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new DomainException("Card number cannot be empty");
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new DomainException("Card holder name cannot be empty");
        if (string.IsNullOrWhiteSpace(expirationDate))
            throw new DomainException("Expiration date cannot be empty");
        if (string.IsNullOrWhiteSpace(securityCode))
            throw new DomainException("Security code cannot be empty");

        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        ExpirationDate = expirationDate;
        SecurityCode = securityCode;
        IsDefault = isDefault;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string cardNumber, string cardHolderName, string expirationDate, string securityCode, bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new DomainException("Card number cannot be empty");
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new DomainException("Card holder name cannot be empty");
        if (string.IsNullOrWhiteSpace(expirationDate))
            throw new DomainException("Expiration date cannot be empty");
        if (string.IsNullOrWhiteSpace(securityCode))
            throw new DomainException("Security code cannot be empty");

        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        ExpirationDate = expirationDate;
        SecurityCode = securityCode;
        IsDefault = isDefault;
        UpdatedAt = DateTime.UtcNow;
    }
}
