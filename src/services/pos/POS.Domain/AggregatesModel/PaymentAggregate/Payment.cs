using POS.Domain.Common;
using POS.Domain.Models;
using POS.Domain.Common.ValueObjects;
using POS.Domain.AggregatesModel.PaymentAggregate.Events;
using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate
{
    public class Payment : Entity
    {
        public Guid SaleId { get; private set; }
        public decimal Amount { get; private set; }
        public Money AmountObj { get; private set; }
        public PaymentMethod Method { get; private set; }
        public string TransactionId { get; private set; }
        public string Currency { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<PaymentEvent> Events { get; private set; }

        private Payment()
        {
            TransactionId = string.Empty;
            Currency = "USD";
            Method = PaymentMethod.Cash;
            AmountObj = new Money(0, "USD");
            Events = new List<PaymentEvent>();
        }

        public Payment(
            Guid saleId,
            decimal amount,
            PaymentMethod method,
            string transactionId,
            string currency)
        {
            SaleId = saleId;
            Amount = amount;
            Method = method;
            TransactionId = transactionId;
            Currency = currency;
            AmountObj = new Money(amount, currency);
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Events = new List<PaymentEvent>();
            AddDomainEvent(new PaymentCreatedDomainEvent(Id, saleId, amount, method));
        }

        public void Process()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Payment is not in pending status");

            Status = PaymentStatus.Processing;
            ProcessedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, PaymentStatus.Processing));
        }

        public void Complete()
        {
            if (Status != PaymentStatus.Processing)
                throw new InvalidOperationException("Payment is not in processing status");

            Status = PaymentStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, PaymentStatus.Completed));
        }

        public void Fail(string reason)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Cannot fail a completed payment");

            Status = PaymentStatus.Failed;
            ErrorMessage = reason;
            ProcessedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, PaymentStatus.Failed));
        }

        public void Refund(string reason)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Can only refund completed payments");

            Status = PaymentStatus.Refunded;
            ErrorMessage = reason;
            ProcessedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, PaymentStatus.Refunded));
        }

        private void AddDomainEvent(IDomainEvent domainEvent)
        {
            // Implementation of AddDomainEvent method
        }
    }

    public class PaymentEvent
    {
        public Guid PaymentId { get; private set; }
        public PaymentEventType Type { get; private set; }
        public string? Details { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private PaymentEvent() { } // For EF Core

        public PaymentEvent(
            Guid paymentId,
            PaymentEventType type,
            string? details = null)
        {
            PaymentId = paymentId;
            Type = type;
            Details = details;
            CreatedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentEventAddedDomainEvent(paymentId, Guid.NewGuid()));
        }
    }

    public enum PaymentStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentEventType
    {
        Processing,
        Completed,
        Failed,
        Refunded
    }
} 
