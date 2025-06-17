using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.PaymentAggregate.Events;

namespace POS.Domain.AggregatesModel.PaymentAggregate
{
    public class Payment : AggregateRoot
    {
        public Guid SaleId { get; private set; }
        public decimal Amount { get; private set; }
        public POS.Domain.Enums.PaymentType Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? Reference { get; private set; }
        public string? CardLast4 { get; private set; }
        public string? CardType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public List<PaymentEvent> Events { get; private set; }

        private Payment()
        {
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
            Events = new List<PaymentEvent>();
        }

        public Payment(Guid saleId, decimal amount, POS.Domain.Enums.PaymentType method, string? reference = null, string? cardLast4 = null, string? cardType = null)
        {
            if (saleId == Guid.Empty)
                throw new DomainException("Sale ID cannot be empty");
            if (amount <= 0)
                throw new DomainException("Amount must be greater than zero");

            SaleId = saleId;
            Amount = amount;
            Method = method;
            Reference = reference;
            CardLast4 = cardLast4;
            CardType = cardType;
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
            Events = new List<PaymentEvent>();

            AddDomainEvent(new PaymentCreatedDomainEvent(Id, saleId, amount, method.ToString()));
        }

        public void Process()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Can only process pending payments");

            Status = PaymentStatus.Processing;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, Status, DateTime.UtcNow));
        }

        public void Complete()
        {
            if (Status != PaymentStatus.Processing)
                throw new DomainException("Can only complete processing payments");

            Status = PaymentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, Status, DateTime.UtcNow));
        }

        public void Fail()
        {
            if (Status == PaymentStatus.Completed)
                throw new DomainException("Cannot fail a completed payment");

            Status = PaymentStatus.Failed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, Status, DateTime.UtcNow));
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Completed)
                throw new DomainException("Can only refund completed payments");

            Status = PaymentStatus.Refunded;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentStatusChangedDomainEvent(Id, Status, DateTime.UtcNow));
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
