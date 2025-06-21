using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate
{
    public class BusinessDocument : Entity
    {
        public string DocumentType { get; private set; } = string.Empty;
        public string DocumentName { get; private set; } = string.Empty;
        public string? DocumentUrl { get; private set; }
        public DateTime UploadDate { get; private set; }
        public bool IsActive { get; private set; }
        public string? Description { get; private set; }
        public long? FileSize { get; private set; }
        public string? FileType { get; private set; }

        protected BusinessDocument() { }

        public BusinessDocument(
            string documentType,
            string documentName,
            string? documentUrl = null,
            string? description = null,
            long? fileSize = null,
            string? fileType = null)
        {
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            DocumentName = documentName ?? throw new ArgumentNullException(nameof(documentName));
            DocumentUrl = documentUrl;
            Description = description;
            FileSize = fileSize;
            FileType = fileType;
            UploadDate = DateTime.UtcNow;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdateDocumentUrl(string? newDocumentUrl)
        {
            DocumentUrl = newDocumentUrl;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        public bool HasValidUrl => !string.IsNullOrEmpty(DocumentUrl);

        public string GetFileExtension()
        {
            if (string.IsNullOrEmpty(DocumentName))
                return string.Empty;

            var lastDotIndex = DocumentName.LastIndexOf('.');
            return lastDotIndex >= 0 ? DocumentName.Substring(lastDotIndex + 1) : string.Empty;
        }
    }
} 
