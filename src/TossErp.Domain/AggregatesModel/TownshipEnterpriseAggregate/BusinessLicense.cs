using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate
{
    public class BusinessLicense : Entity
    {
        public string LicenseType { get; private set; } = string.Empty;
        public string LicenseNumber { get; private set; } = string.Empty;
        public DateTime IssueDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsActive { get; private set; }
        public string? IssuingAuthority { get; private set; }
        public string? Notes { get; private set; }

        protected BusinessLicense() { }

        public BusinessLicense(
            string licenseType,
            string licenseNumber,
            DateTime issueDate,
            DateTime expiryDate,
            string? issuingAuthority = null,
            string? notes = null)
        {
            LicenseType = licenseType ?? throw new ArgumentNullException(nameof(licenseType));
            LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            IssuingAuthority = issuingAuthority;
            Notes = notes;
            IsActive = true;

            if (expiryDate <= issueDate)
                throw new ArgumentException("Expiry date must be after issue date.");
        }

        public bool IsValid => IsActive && DateTime.UtcNow <= ExpiryDate;

        public bool IsExpired => DateTime.UtcNow > ExpiryDate;

        public bool IsExpiringSoon(int daysThreshold = 30)
        {
            return IsActive && DateTime.UtcNow.AddDays(daysThreshold) >= ExpiryDate;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdateExpiryDate(DateTime newExpiryDate)
        {
            if (newExpiryDate <= IssueDate)
                throw new ArgumentException("Expiry date must be after issue date.");

            ExpiryDate = newExpiryDate;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
    }
} 
