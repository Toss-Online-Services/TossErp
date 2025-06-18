using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record CustomerPreferences
{
    public bool ReceiveEmailNotifications { get; }
    public bool ReceiveSMSNotifications { get; }
    public bool ReceivePostalMail { get; }
    public string PreferredLanguage { get; }
    public string PreferredCurrency { get; }
    public string PreferredPaymentMethod { get; }
    public string PreferredShippingMethod { get; }
    public bool OptInMarketing { get; }
    public bool OptInThirdParty { get; }
    public string? DietaryRestrictions { get; }
    public string? SpecialInstructions { get; }

    public CustomerPreferences(
        bool receiveEmailNotifications = true,
        bool receiveSMSNotifications = true,
        bool receivePostalMail = false,
        string preferredLanguage = "en",
        string preferredCurrency = "USD",
        string preferredPaymentMethod = "Credit Card",
        string preferredShippingMethod = "Standard",
        bool optInMarketing = false,
        bool optInThirdParty = false,
        string? dietaryRestrictions = null,
        string? specialInstructions = null)
    {
        if (string.IsNullOrWhiteSpace(preferredLanguage))
            throw new DomainException("Preferred language cannot be empty");
        if (string.IsNullOrWhiteSpace(preferredCurrency))
            throw new DomainException("Preferred currency cannot be empty");
        if (string.IsNullOrWhiteSpace(preferredPaymentMethod))
            throw new DomainException("Preferred payment method cannot be empty");
        if (string.IsNullOrWhiteSpace(preferredShippingMethod))
            throw new DomainException("Preferred shipping method cannot be empty");

        ReceiveEmailNotifications = receiveEmailNotifications;
        ReceiveSMSNotifications = receiveSMSNotifications;
        ReceivePostalMail = receivePostalMail;
        PreferredLanguage = preferredLanguage.ToLowerInvariant();
        PreferredCurrency = preferredCurrency.ToUpperInvariant();
        PreferredPaymentMethod = preferredPaymentMethod;
        PreferredShippingMethod = preferredShippingMethod;
        OptInMarketing = optInMarketing;
        OptInThirdParty = optInThirdParty;
        DietaryRestrictions = dietaryRestrictions;
        SpecialInstructions = specialInstructions;
    }

    public static CustomerPreferences Create(
        bool receiveEmailNotifications = true,
        bool receiveSMSNotifications = true,
        bool receivePostalMail = false,
        string preferredLanguage = "en",
        string preferredCurrency = "USD",
        string preferredPaymentMethod = "Credit Card",
        string preferredShippingMethod = "Standard",
        bool optInMarketing = false,
        bool optInThirdParty = false,
        string? dietaryRestrictions = null,
        string? specialInstructions = null) =>
        new(
            receiveEmailNotifications,
            receiveSMSNotifications,
            receivePostalMail,
            preferredLanguage,
            preferredCurrency,
            preferredPaymentMethod,
            preferredShippingMethod,
            optInMarketing,
            optInThirdParty,
            dietaryRestrictions,
            specialInstructions);

    public CustomerPreferences WithUpdatedPreferences(
        bool? receiveEmailNotifications = null,
        bool? receiveSMSNotifications = null,
        bool? receivePostalMail = null,
        string? preferredLanguage = null,
        string? preferredCurrency = null,
        string? preferredPaymentMethod = null,
        string? preferredShippingMethod = null,
        bool? optInMarketing = null,
        bool? optInThirdParty = null,
        string? dietaryRestrictions = null,
        string? specialInstructions = null) =>
        new(
            receiveEmailNotifications ?? ReceiveEmailNotifications,
            receiveSMSNotifications ?? ReceiveSMSNotifications,
            receivePostalMail ?? ReceivePostalMail,
            preferredLanguage ?? PreferredLanguage,
            preferredCurrency ?? PreferredCurrency,
            preferredPaymentMethod ?? PreferredPaymentMethod,
            preferredShippingMethod ?? PreferredShippingMethod,
            optInMarketing ?? OptInMarketing,
            optInThirdParty ?? OptInThirdParty,
            dietaryRestrictions ?? DietaryRestrictions,
            specialInstructions ?? SpecialInstructions);
} 
