#nullable enable
using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        public int BuyerId { get; private set; }
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public string CardType { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected PaymentMethod()
        {
            CardNumber = string.Empty;
            CardHolderName = string.Empty;
            CardType = string.Empty;
            ExpirationDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public PaymentMethod(int buyerId, string cardNumber, string cardHolderName, DateTime expirationDate, string cardType, bool isDefault = false)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new DomainException("Card number cannot be empty");
            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new DomainException("Card holder name cannot be empty");
            if (string.IsNullOrWhiteSpace(cardType))
                throw new DomainException("Card type cannot be empty");

            BuyerId = buyerId;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            ExpirationDate = expirationDate;
            CardType = cardType;
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string cardNumber, string cardHolderName, DateTime expirationDate, string cardType, bool isDefault)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new DomainException("Card number cannot be empty");
            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new DomainException("Card holder name cannot be empty");
            if (string.IsNullOrWhiteSpace(cardType))
                throw new DomainException("Card type cannot be empty");

            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            ExpirationDate = expirationDate;
            CardType = cardType;
            IsDefault = isDefault;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
