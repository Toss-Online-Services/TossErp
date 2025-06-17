using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events
{
    public class CustomerPreferencesChangedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid PreferencesId { get; }
        public string ChangedField { get; }
        public string? OldValue { get; }
        public string? NewValue { get; }
        public DateTime ChangedAt { get; }

        public CustomerPreferencesChangedEvent(
            Guid customerId,
            Guid preferencesId,
            string changedField,
            string? oldValue,
            string? newValue)
        {
            CustomerId = customerId;
            PreferencesId = preferencesId;
            ChangedField = changedField;
            OldValue = oldValue;
            NewValue = newValue;
            ChangedAt = DateTime.UtcNow;
        }
    }
} 
