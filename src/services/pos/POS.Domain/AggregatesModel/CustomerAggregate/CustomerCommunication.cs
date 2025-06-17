using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerCommunication : Entity
    {
        public string CommunicationType { get; private set; }
        public string Subject { get; private set; }
        public string Content { get; private set; }
        public string? Recipient { get; private set; }
        public string? Sender { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public string? ResponseContent { get; private set; }
        public DateTime? ResponseAt { get; private set; }
        public string? CommunicationChannel { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public bool IsOutbound { get; private set; }
        public string? Tags { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private CustomerCommunication() { }

        public CustomerCommunication(
            string communicationType,
            string subject,
            string content,
            string? recipient = null,
            string? sender = null,
            string? communicationChannel = null,
            string? referenceNumber = null,
            bool isOutbound = true,
            string? tags = null)
        {
            if (string.IsNullOrWhiteSpace(communicationType))
                throw new DomainException("Communication type cannot be empty");
            if (string.IsNullOrWhiteSpace(subject))
                throw new DomainException("Subject cannot be empty");
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Content cannot be empty");

            Id = Guid.NewGuid();
            CommunicationType = communicationType;
            Subject = subject;
            Content = content;
            Recipient = recipient;
            Sender = sender;
            CommunicationChannel = communicationChannel;
            ReferenceNumber = referenceNumber;
            IsOutbound = isOutbound;
            Tags = tags;
            SentAt = DateTime.UtcNow;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
                LastModifiedAt = DateTime.UtcNow;
            }
        }

        public void AddResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
                throw new DomainException("Response content cannot be empty");

            ResponseContent = responseContent;
            ResponseAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateTags(string? tags)
        {
            Tags = tags;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool HasResponse => ResponseContent != null && ResponseAt.HasValue;
        public TimeSpan? ResponseTime => ResponseAt?.Subtract(SentAt);
    }
} 
