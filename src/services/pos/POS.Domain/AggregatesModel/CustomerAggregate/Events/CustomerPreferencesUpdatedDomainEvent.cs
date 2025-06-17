using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerPreferencesUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public bool ReceiveEmailNotifications { get; }
    public bool ReceiveSMSNotifications { get; }
    public bool ReceivePostalMail { get; }
    public string PreferredLanguage { get; }
    public string PreferredCurrency { get; }
    public string PreferredPaymentMethod { get; }
    public string DietaryRestrictions { get; }
    public DateTime UpdatedAt { get; }

    public CustomerPreferencesUpdatedDomainEvent(
        Guid customerId,
        bool receiveEmailNotifications,
        bool receiveSMSNotifications,
        bool receivePostalMail,
        string preferredLanguage,
        string preferredCurrency,
        string preferredPaymentMethod,
        string dietaryRestrictions,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        ReceiveEmailNotifications = receiveEmailNotifications;
        ReceiveSMSNotifications = receiveSMSNotifications;
        ReceivePostalMail = receivePostalMail;
        PreferredLanguage = preferredLanguage;
        PreferredCurrency = preferredCurrency;
        PreferredPaymentMethod = preferredPaymentMethod;
        DietaryRestrictions = dietaryRestrictions;
        UpdatedAt = updatedAt;
    }
} 
