using POS.Domain.Common;
using POS.Domain.Enums;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.PaymentAggregate
{
    public class PaymentSplit : ValueObject
    {
        public decimal Amount { get; private set; }
        public PaymentType Method { get; private set; }
        public string? Reference { get; private set; }
        public string? CardLast4 { get; private set; }
        public string? CardType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private PaymentSplit() { }

        public PaymentSplit(decimal amount, PaymentType method, string? reference = null, 
            string? cardLast4 = null, string? cardType = null)
        {
            if (amount <= 0)
                throw new DomainException("Split amount must be greater than zero");

            Amount = amount;
            Method = method;
            Reference = reference;
            CardLast4 = cardLast4;
            CardType = cardType;
            CreatedAt = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Method;
            yield return Reference ?? string.Empty;
            yield return CardLast4 ?? string.Empty;
            yield return CardType ?? string.Empty;
            yield return CreatedAt;
        }
    }
} 
