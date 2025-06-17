using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerDocument : Entity
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public string FileType { get; private set; }
        public long FileSize { get; private set; }
        public string? Description { get; private set; }
        public string UploadedBy { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public bool IsRequired { get; private set; }
        public bool IsVerified { get; private set; }
        public string? VerificationNotes { get; private set; }
        public DateTime? VerifiedAt { get; private set; }
        public string? VerifiedBy { get; private set; }

        private CustomerDocument()
        {
            Name = string.Empty;
            FilePath = string.Empty;
            FileType = string.Empty;
            FileSize = 0;
            UploadedBy = string.Empty;
            UploadedAt = DateTime.UtcNow;
            IsRequired = false;
            IsVerified = false;
        }

        public CustomerDocument(string name, string filePath, string fileType, long fileSize, 
            string uploadedBy, string? description = null, DateTime? expiryDate = null, bool isRequired = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Document name cannot be empty");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainException("File path cannot be empty");
            if (string.IsNullOrWhiteSpace(fileType))
                throw new DomainException("File type cannot be empty");
            if (fileSize <= 0)
                throw new DomainException("File size must be greater than zero");
            if (string.IsNullOrWhiteSpace(uploadedBy))
                throw new DomainException("Uploader cannot be empty");

            Id = Guid.NewGuid();
            Name = name;
            FilePath = filePath;
            FileType = fileType;
            FileSize = fileSize;
            Description = description;
            UploadedBy = uploadedBy;
            UploadedAt = DateTime.UtcNow;
            ExpiryDate = expiryDate;
            IsRequired = isRequired;
            IsVerified = false;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateExpiryDate(DateTime? expiryDate)
        {
            ExpiryDate = expiryDate;
        }

        public void Verify(string verifiedBy, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(verifiedBy))
                throw new DomainException("Verifier cannot be empty");

            IsVerified = true;
            VerificationNotes = notes;
            VerifiedAt = DateTime.UtcNow;
            VerifiedBy = verifiedBy;
        }

        public void Unverify()
        {
            IsVerified = false;
            VerificationNotes = null;
            VerifiedAt = null;
            VerifiedBy = null;
        }

        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }
} 
