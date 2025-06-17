using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events
{
    public class CustomerCommunicationSentEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid CommunicationId { get; }
        public string CommunicationType { get; }
        public string Subject { get; }
        public string? Recipient { get; }
        public string? Sender { get; }
        public string? CommunicationChannel { get; }
        public bool IsOutbound { get; }
        public DateTime SentAt { get; }

        public CustomerCommunicationSentEvent(
            Guid customerId,
            Guid communicationId,
            string communicationType,
            string subject,
            string? recipient = null,
            string? sender = null,
            string? communicationChannel = null,
            bool isOutbound = true)
        {
            CustomerId = customerId;
            CommunicationId = communicationId;
            CommunicationType = communicationType;
            Subject = subject;
            Recipient = recipient;
            Sender = sender;
            CommunicationChannel = communicationChannel;
            IsOutbound = isOutbound;
            SentAt = DateTime.UtcNow;
        }
    }
} 
