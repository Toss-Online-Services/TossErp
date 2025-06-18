using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.ValueObjects;
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
        public PaymentType Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? Reference { get; private set; }
        public string? CardLast4 { get; private set; }
        public string? CardType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public List<PaymentEvent> Events { get; private set; }
        
        // New properties for enhanced payment processing
        public decimal PartialRefundAmount { get; private set; }
        public List<PaymentSplit> PaymentSplits { get; private set; }
        public PaymentGatewayResponse? GatewayResponse { get; private set; }
        public string? TransactionId { get; private set; }
        public string? AuthorizationCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime? LastRetryAt { get; private set; }
        public bool IsReconciled { get; private set; }
        public DateTime? ReconciledAt { get; private set; }
        public string? ReconciliationReference { get; private set; }

        private Payment()
        {
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
            Events = new List<PaymentEvent>();
            PaymentSplits = new List<PaymentSplit>();
            RetryCount = 0;
            IsReconciled = false;
        }

        public Payment(Guid saleId, decimal amount, PaymentType method, string? reference = null, 
            string? cardLast4 = null, string? cardType = null) : this()
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

            AddDomainEvent(new PaymentCreatedDomainEvent(Id, saleId, amount, method.ToString()));
        }

        public void Process()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Can only process pending payments");

            Status = PaymentStatus.Processing;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentProcessingDomainEvent(Id, SaleId, Amount, Method.ToString()));
        }

        public void Complete(string transactionId, string authorizationCode)
        {
            if (Status != PaymentStatus.Processing)
                throw new DomainException("Can only complete processing payments");

            Status = PaymentStatus.Completed;
            TransactionId = transactionId;
            AuthorizationCode = authorizationCode;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentCompletedDomainEvent(Id, SaleId, Amount, Method.ToString(), transactionId));
        }

        public void Fail(string errorMessage)
        {
            if (Status != PaymentStatus.Processing)
                throw new DomainException("Can only fail processing payments");

            Status = PaymentStatus.Failed;
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentFailedDomainEvent(Id, SaleId, Amount, Method.ToString(), errorMessage));
        }

        public void Retry()
        {
            if (Status != PaymentStatus.Failed)
                throw new DomainException("Can only retry failed payments");
            if (RetryCount >= 3)
                throw new DomainException("Maximum retry attempts reached");
            Status = PaymentStatus.Pending;
            RetryCount++;
            LastRetryAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new PaymentRetryDomainEvent(Id, RetryCount, "System", LastRetryAt.Value));
        }

        public void ProcessPartialRefund(decimal refundAmount, string reason)
        {
            if (Status != PaymentStatus.Completed)
                throw new DomainException("Can only refund completed payments");

            if (refundAmount <= 0)
                throw new DomainException("Refund amount must be greater than zero");

            if (refundAmount > Amount - PartialRefundAmount)
                throw new DomainException("Refund amount cannot exceed remaining amount");

            PartialRefundAmount += refundAmount;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentRefundedDomainEvent(Id, refundAmount, "System", UpdatedAt.Value, reason));
        }

        public void AddPaymentSplit(PaymentSplit split)
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Can only add splits to pending payments");

            if (PaymentSplits.Sum(s => s.Amount) + split.Amount > Amount)
                throw new DomainException("Total split amount cannot exceed payment amount");

            PaymentSplits.Add(split);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentSplitAddedDomainEvent(Id, SaleId, split.Amount, split.Method.ToString()));
        }

        public void Reconcile(string reference)
        {
            if (Status != PaymentStatus.Completed)
                throw new DomainException("Can only reconcile completed payments");

            IsReconciled = true;
            ReconciledAt = DateTime.UtcNow;
            ReconciliationReference = reference;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentReconciledDomainEvent(Id, SaleId, reference));
        }

        public void UpdateGatewayResponse(PaymentGatewayResponse response)
        {
            GatewayResponse = response;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentGatewayResponseUpdatedDomainEvent(Id, SaleId, response.Status));
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
