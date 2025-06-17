using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerPreferences : Entity
    {
        public bool ReceiveEmailNotifications { get; private set; }
        public bool ReceiveSMSNotifications { get; private set; }
        public bool ReceivePostalMail { get; private set; }
        public string PreferredLanguage { get; private set; }
        public string PreferredCurrency { get; private set; }
        public string? PreferredPaymentMethod { get; private set; }
        public string? PreferredShippingMethod { get; private set; }
        public bool OptInMarketing { get; private set; }
        public bool OptInThirdParty { get; private set; }
        public string? DietaryRestrictions { get; private set; }
        public string? SpecialInstructions { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private CustomerPreferences() { }

        public CustomerPreferences(
            string preferredLanguage = "en",
            string preferredCurrency = "USD",
            bool receiveEmailNotifications = true,
            bool receiveSMSNotifications = false,
            bool receivePostalMail = false,
            bool optInMarketing = false,
            bool optInThirdParty = false)
        {
            if (string.IsNullOrWhiteSpace(preferredLanguage))
                throw new DomainException("Preferred language cannot be empty");
            if (string.IsNullOrWhiteSpace(preferredCurrency))
                throw new DomainException("Preferred currency cannot be empty");

            Id = Guid.NewGuid();
            PreferredLanguage = preferredLanguage;
            PreferredCurrency = preferredCurrency;
            ReceiveEmailNotifications = receiveEmailNotifications;
            ReceiveSMSNotifications = receiveSMSNotifications;
            ReceivePostalMail = receivePostalMail;
            OptInMarketing = optInMarketing;
            OptInThirdParty = optInThirdParty;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateNotificationPreferences(
            bool? emailNotifications = null,
            bool? smsNotifications = null,
            bool? postalMail = null)
        {
            if (emailNotifications.HasValue)
                ReceiveEmailNotifications = emailNotifications.Value;
            if (smsNotifications.HasValue)
                ReceiveSMSNotifications = smsNotifications.Value;
            if (postalMail.HasValue)
                ReceivePostalMail = postalMail.Value;

            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new DomainException("Language cannot be empty");

            PreferredLanguage = language;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Currency cannot be empty");

            PreferredCurrency = currency;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetPreferredPaymentMethod(string? paymentMethod)
        {
            PreferredPaymentMethod = paymentMethod;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetPreferredShippingMethod(string? shippingMethod)
        {
            PreferredShippingMethod = shippingMethod;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateMarketingPreferences(
            bool? optInMarketing = null,
            bool? optInThirdParty = null)
        {
            if (optInMarketing.HasValue)
                OptInMarketing = optInMarketing.Value;
            if (optInThirdParty.HasValue)
                OptInThirdParty = optInThirdParty.Value;

            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetDietaryRestrictions(string? restrictions)
        {
            DietaryRestrictions = restrictions;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetSpecialInstructions(string? instructions)
        {
            SpecialInstructions = instructions;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool HasAnyNotificationsEnabled => 
            ReceiveEmailNotifications || ReceiveSMSNotifications || ReceivePostalMail;
    }
} 
