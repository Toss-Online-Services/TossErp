using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate
{
    public class BusinessContact : Entity
    {
        public string ContactName { get; private set; } = string.Empty;
        public string ContactNumber { get; private set; } = string.Empty;
        public string? EmailAddress { get; private set; }
        public string? Relationship { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public string? Notes { get; private set; }

        protected BusinessContact() { }

        public BusinessContact(
            string contactName,
            string contactNumber,
            string? emailAddress = null,
            string? relationship = null,
            string? notes = null)
        {
            ContactName = contactName ?? throw new ArgumentNullException(nameof(contactName));
            ContactNumber = contactNumber ?? throw new ArgumentNullException(nameof(contactNumber));
            EmailAddress = emailAddress;
            Relationship = relationship;
            Notes = notes;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateContact(
            string contactName,
            string contactNumber,
            string? emailAddress = null,
            string? relationship = null)
        {
            ContactName = contactName ?? throw new ArgumentNullException(nameof(contactName));
            ContactNumber = contactNumber ?? throw new ArgumentNullException(nameof(contactNumber));
            EmailAddress = emailAddress;
            Relationship = relationship;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool HasValidEmail => !string.IsNullOrEmpty(EmailAddress) && EmailAddress.Contains("@");

        public bool HasValidPhone => !string.IsNullOrEmpty(ContactNumber) && ContactNumber.Length >= 10;
    }
} 
