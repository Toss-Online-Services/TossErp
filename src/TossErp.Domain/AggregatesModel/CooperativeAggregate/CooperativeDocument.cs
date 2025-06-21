using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class CooperativeDocument : Entity
    {
        public string DocumentType { get; private set; } = string.Empty;
        public string DocumentNumber { get; private set; } = string.Empty;
        public DateTime IssueDate { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public string? DocumentUrl { get; private set; }
        public bool IsValid { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }

        protected CooperativeDocument() { }

        public CooperativeDocument(
            string documentType,
            string documentNumber,
            DateTime issueDate,
            DateTime? expiryDate = null,
            string? documentUrl = null)
        {
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            DocumentUrl = documentUrl;
            IsValid = true;
            CreatedDate = DateTime.UtcNow;
        }

        public void UpdateDocumentUrl(string? newDocumentUrl)
        {
            DocumentUrl = newDocumentUrl;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void UpdateExpiryDate(DateTime? newExpiryDate)
        {
            ExpiryDate = newExpiryDate;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsValid = false;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsValid = true;
            LastModifiedDate = DateTime.UtcNow;
        }

        public bool IsExpired => ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;

        public bool IsExpiringSoon(int daysThreshold = 30)
        {
            return ExpiryDate.HasValue && DateTime.UtcNow.AddDays(daysThreshold) >= ExpiryDate.Value;
        }

        public bool HasValidUrl => !string.IsNullOrEmpty(DocumentUrl);
    }
} 
