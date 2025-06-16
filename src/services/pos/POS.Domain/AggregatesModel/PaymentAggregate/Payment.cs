using POS.Domain.Common;
using POS.Domain.Models;

namespace POS.Domain.AggregatesModel.PaymentAggregate
{
    public class Payment : Entity
    {
        public string TransactionId { get; private set; }
        public Guid StoreId { get; private set; }
        public Guid? SaleId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public PaymentMethod Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? Reference { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<PaymentEvent> Events { get; private set; }

        private Payment() { } // For EF Core

        public Payment(
            string transactionId,
            Guid storeId,
            decimal amount,
            string currency,
            PaymentMethod method,
            Guid? saleId = null,
            string? reference = null,
            string? notes = null)
        {
            TransactionId = transactionId;
            StoreId = storeId;
            Amount = amount;
            Currency = currency;
            Method = method;
            SaleId = saleId;
            Reference = reference;
            Notes = notes;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Events = new List<PaymentEvent>();
        }

        public void Process()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Payment is not in pending status");

            Status = PaymentStatus.Processing;
            LastModifiedAt = DateTime.UtcNow;
            Events.Add(new PaymentEvent(Id, PaymentEventType.Processing));
        }

        public void Complete()
        {
            if (Status != PaymentStatus.Processing)
                throw new InvalidOperationException("Payment is not in processing status");

            Status = PaymentStatus.Completed;
            LastModifiedAt = DateTime.UtcNow;
            Events.Add(new PaymentEvent(Id, PaymentEventType.Completed));
        }

        public void Fail(string reason)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Cannot fail a completed payment");

            Status = PaymentStatus.Failed;
            Notes = reason;
            LastModifiedAt = DateTime.UtcNow;
            Events.Add(new PaymentEvent(Id, PaymentEventType.Failed, reason));
        }

        public void Refund(string reason)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Can only refund completed payments");

            Status = PaymentStatus.Refunded;
            Notes = reason;
            LastModifiedAt = DateTime.UtcNow;
            Events.Add(new PaymentEvent(Id, PaymentEventType.Refunded, reason));
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
