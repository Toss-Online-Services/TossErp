using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class ContactInfo : ValueObject
    {
        public string PhoneNumber { get; private set; } = string.Empty;
        public string? EmailAddress { get; private set; }
        public string? WhatsAppNumber { get; private set; }
        public string? Website { get; private set; }

        public ContactInfo(string phoneNumber, string? emailAddress = null, string? whatsAppNumber = null, string? website = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            PhoneNumber = phoneNumber.Trim();
            EmailAddress = emailAddress?.Trim();
            WhatsAppNumber = whatsAppNumber?.Trim();
            Website = website?.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PhoneNumber;
            yield return EmailAddress ?? string.Empty;
            yield return WhatsAppNumber ?? string.Empty;
            yield return Website ?? string.Empty;
        }

        public override string ToString()
        {
            var parts = new List<string> { PhoneNumber };
            if (!string.IsNullOrWhiteSpace(EmailAddress))
                parts.Add(EmailAddress);
            if (!string.IsNullOrWhiteSpace(WhatsAppNumber))
                parts.Add($"WhatsApp: {WhatsAppNumber}");
            if (!string.IsNullOrWhiteSpace(Website))
                parts.Add(Website);
            
            return string.Join(" | ", parts);
        }
    }
} 
