#nullable enable
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        public Guid BuyerId { get; private set; }
        public Buyer Buyer { get; private set; } = null!;
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public string ExpirationDate { get; private set; }
        public string? SecurityCode { get; private set; }
        public string? Alias { get; private set; }
        public Guid CardTypeId { get; private set; }
        public CardType CardType { get; private set; } = null!;
        public bool IsDefault { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected PaymentMethod()
        {
            CardNumber = string.Empty;
            CardHolderName = string.Empty;
            ExpirationDate = string.Empty;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public PaymentMethod(Guid buyerId, string cardNumber, string cardHolderName, string expirationDate,
            Guid cardTypeId, string? securityCode = null, string? alias = null)
        {
            if (buyerId == Guid.Empty)
                throw new DomainException("Buyer ID cannot be empty");
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new DomainException("Card number cannot be empty");
            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new DomainException("Card holder name cannot be empty");
            if (string.IsNullOrWhiteSpace(expirationDate))
                throw new DomainException("Expiration date cannot be empty");
            if (cardTypeId == Guid.Empty)
                throw new DomainException("Card type ID cannot be empty");

            BuyerId = buyerId;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;
            Alias = alias;
            CardTypeId = cardTypeId;
            IsDefault = false;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string cardHolderName, string expirationDate, string? securityCode = null, string? alias = null)
        {
            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new DomainException("Card holder name cannot be empty");
            if (string.IsNullOrWhiteSpace(expirationDate))
                throw new DomainException("Expiration date cannot be empty");

            CardHolderName = cardHolderName;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;
            Alias = alias;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            if (IsDefault)
                throw new DomainException("Payment method is already set as default");

            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveDefault()
        {
            if (!IsDefault)
                throw new DomainException("Payment method is not set as default");

            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Payment method is already inactive");

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("Payment method is already active");

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            if (string.IsNullOrWhiteSpace(ExpirationDate))
                return false;

            if (DateTime.TryParse(ExpirationDate, out DateTime expirationDate))
            {
                return expirationDate < DateTime.UtcNow;
            }

            return false;
        }

        public bool IsEqualTo(PaymentMethod other)
        {
            if (other == null)
                return false;

            return CardNumber == other.CardNumber &&
                   CardHolderName == other.CardHolderName &&
                   ExpirationDate == other.ExpirationDate &&
                   CardTypeId == other.CardTypeId;
        }
    }
}
