using TossErp.CRM.Domain.Entities;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Events;
using TossErp.CRM.Domain.SeedWork;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Aggregates;

/// <summary>
/// Customer aggregate representing customers in the CRM system
/// Handles customer lifecycle, subscription management, and relationship tracking
/// </summary>
public class Customer : AggregateRoot
{
    private readonly List<Contact> _contacts;
    private readonly List<Activity> _activities;
    private readonly List<Note> _notes;
    private readonly List<Communication> _communications;
    private readonly List<Document> _documents;

    // Core properties
    public Guid TenantId { get; private set; }
    public CustomerNumber CustomerNumber { get; private set; }
    public string Name { get; private set; }
    public CustomerType Type { get; private set; }
    public CustomerStatus Status { get; private set; }
    public CustomerTier Tier { get; private set; }
    public LeadSource? Source { get; private set; }

    // Contact information
    public EmailAddress? PrimaryEmail { get; private set; }
    public PhoneNumber? PrimaryPhone { get; private set; }
    public Address? BillingAddress { get; private set; }
    public Address? ShippingAddress { get; private set; }

    // Business information
    public string? Industry { get; private set; }
    public int? EmployeeCount { get; private set; }
    public Money? AnnualRevenue { get; private set; }
    public string? Website { get; private set; }
    public string? TaxId { get; private set; }

    // Relationship tracking
    public string? AssignedTo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    // SaaS-specific
    public SubscriptionStatus SubscriptionStatus { get; private set; }
    public DateTime? SubscriptionStartDate { get; private set; }
    public DateTime? SubscriptionEndDate { get; private set; }
    public Money? MonthlyRecurringRevenue { get; private set; }
    public DateTime? LastActivityDate { get; private set; }
    public LeadScore CustomerScore { get; private set; }

    // Collections
    public IReadOnlyList<Contact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();
    public IReadOnlyList<Note> Notes => _notes.AsReadOnly();
    public IReadOnlyList<Communication> Communications => _communications.AsReadOnly();
    public IReadOnlyList<Document> Documents => _documents.AsReadOnly();

    private Customer() 
    { 
        _contacts = new List<Contact>();
        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();
        _documents = new List<Document>();
    } // EF Core

    public Customer(
        Guid id,
        Guid tenantId,
        CustomerNumber customerNumber,
        string name,
        CustomerType type,
        string createdBy,
        LeadSource? source = null,
        EmailAddress? primaryEmail = null,
        PhoneNumber? primaryPhone = null,
        string? industry = null)
    {
        Id = id;
        TenantId = tenantId;
        CustomerNumber = customerNumber ?? throw new ArgumentNullException(nameof(customerNumber));
        Name = name?.Trim() ?? throw new ArgumentException("Customer name cannot be empty");
        Type = type;
        Status = CustomerStatus.Prospect;
        Tier = CustomerTier.Standard;
        Source = source;
        PrimaryEmail = primaryEmail;
        PrimaryPhone = primaryPhone;
        Industry = industry?.Trim();
        AssignedTo = null;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        SubscriptionStatus = SubscriptionStatus.Trial;
        CustomerScore = new LeadScore(50); // Default score

        _contacts = new List<Contact>();
        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();
        _documents = new List<Document>();

        AddDomainEvent(new CustomerCreatedEvent(tenantId, id, name, type, createdBy));
    }

    #region Customer Management

    public void UpdateBasicInfo(
        string name,
        string modifiedBy,
        CustomerType? type = null,
        string? industry = null,
        int? employeeCount = null,
        Money? annualRevenue = null,
        string? website = null,
        string? taxId = null)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Customer name cannot be empty");
        
        if (type.HasValue && type.Value != Type)
            Type = type.Value;

        Industry = industry?.Trim();
        EmployeeCount = employeeCount;
        AnnualRevenue = annualRevenue;
        Website = website?.Trim();
        TaxId = taxId?.Trim();
        
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateContactInfo(EmailAddress? primaryEmail, PhoneNumber? primaryPhone, string modifiedBy)
    {
        PrimaryEmail = primaryEmail;
        PrimaryPhone = primaryPhone;
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateBillingAddress(Address address, string modifiedBy)
    {
        BillingAddress = address;
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateShippingAddress(Address address, string modifiedBy)
    {
        ShippingAddress = address;
        UpdateModificationInfo(modifiedBy);
    }

    public void ChangeStatus(CustomerStatus newStatus, string changedBy, string? reason = null)
    {
        if (Status == newStatus)
            return;

        var previousStatus = Status;
        Status = newStatus;
        UpdateModificationInfo(changedBy);

        AddDomainEvent(new CustomerStatusChangedEvent(TenantId, Id, previousStatus, newStatus, changedBy, reason));
    }

    public void UpgradeTier(CustomerTier newTier, string upgradedBy, Money? newAnnualValue = null)
    {
        if (newTier <= Tier)
            throw new InvalidOperationException($"Cannot downgrade or maintain same tier. Current: {Tier}, Requested: {newTier}");

        var previousTier = Tier;
        Tier = newTier;
        
        if (newAnnualValue != null)
            AnnualRevenue = newAnnualValue;

        UpdateModificationInfo(upgradedBy);

        AddDomainEvent(new CustomerUpgradedEvent(TenantId, Id, previousTier, newTier, upgradedBy, newAnnualValue));
    }

    public void Assign(string assignedTo, string assignedBy)
    {
        var previousAssignee = AssignedTo;
        AssignedTo = assignedTo?.Trim() ?? throw new ArgumentNullException(nameof(assignedTo));
        UpdateModificationInfo(assignedBy);

        // Could add domain event for assignment if needed
    }

    public void Unassign(string unassignedBy)
    {
        AssignedTo = null;
        UpdateModificationInfo(unassignedBy);
    }

    #endregion

    #region Subscription Management

    public void StartSubscription(DateTime startDate, DateTime endDate, Money monthlyRecurringRevenue, string startedBy)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");

        SubscriptionStatus = SubscriptionStatus.Active;
        SubscriptionStartDate = startDate;
        SubscriptionEndDate = endDate;
        MonthlyRecurringRevenue = monthlyRecurringRevenue;
        
        // Auto-promote from prospect to active customer
        if (Status == CustomerStatus.Prospect)
            ChangeStatus(CustomerStatus.Active, startedBy, "Subscription started");

        UpdateModificationInfo(startedBy);
    }

    public void RenewSubscription(DateTime newEndDate, Money renewalAmount, string renewedBy)
    {
        if (SubscriptionStatus != SubscriptionStatus.Active && SubscriptionStatus != SubscriptionStatus.Expired)
            throw new InvalidOperationException("Can only renew active or expired subscriptions");

        if (newEndDate <= DateTime.UtcNow)
            throw new ArgumentException("New end date must be in the future");

        SubscriptionEndDate = newEndDate;
        SubscriptionStatus = SubscriptionStatus.Active;
        UpdateModificationInfo(renewedBy);

        AddDomainEvent(new CustomerSubscriptionRenewedEvent(TenantId, Id, newEndDate, renewalAmount, renewedBy));
    }

    public void SuspendSubscription(string suspendedBy, string reason)
    {
        if (SubscriptionStatus != SubscriptionStatus.Active)
            throw new InvalidOperationException("Can only suspend active subscriptions");

        SubscriptionStatus = SubscriptionStatus.Suspended;
        ChangeStatus(CustomerStatus.Inactive, suspendedBy, $"Subscription suspended: {reason}");
        UpdateModificationInfo(suspendedBy);
    }

    public void CancelSubscription(string cancelledBy, string reason)
    {
        SubscriptionStatus = SubscriptionStatus.Cancelled;
        ChangeStatus(CustomerStatus.Inactive, cancelledBy, $"Subscription cancelled: {reason}");
        UpdateModificationInfo(cancelledBy);
    }

    public bool IsSubscriptionActive => SubscriptionStatus == SubscriptionStatus.Active && 
                                      SubscriptionEndDate.HasValue && 
                                      SubscriptionEndDate.Value > DateTime.UtcNow;

    public bool IsSubscriptionExpiring => IsSubscriptionActive && 
                                         SubscriptionEndDate.HasValue && 
                                         SubscriptionEndDate.Value <= DateTime.UtcNow.AddDays(30);

    #endregion

    #region Contact Management

    public void AddContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        // If this is the first contact or marked as primary, update customer primary contact info
        if (!_contacts.Any() || contact.IsPrimary)
        {
            // Remove primary flag from existing contacts
            foreach (var existingContact in _contacts.Where(c => c.IsPrimary))
            {
                existingContact.RemoveAsPrimary();
            }

            // Update customer primary contact info
            PrimaryEmail = contact.Email;
            PrimaryPhone = contact.Phone;
        }

        _contacts.Add(contact);
        RecordActivity();
    }

    public void RemoveContact(Guid contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null)
            return;

        _contacts.Remove(contact);

        // If removed contact was primary, find a new primary or clear
        if (contact.IsPrimary)
        {
            var newPrimary = _contacts.FirstOrDefault(c => c.IsActive);
            if (newPrimary != null)
            {
                newPrimary.SetAsPrimary();
                PrimaryEmail = newPrimary.Email;
                PrimaryPhone = newPrimary.Phone;
            }
            else
            {
                PrimaryEmail = null;
                PrimaryPhone = null;
            }
        }

        RecordActivity();
    }

    public Contact? GetPrimaryContact()
    {
        return _contacts.FirstOrDefault(c => c.IsPrimary && c.IsActive);
    }

    public IEnumerable<Contact> GetActiveContacts()
    {
        return _contacts.Where(c => c.IsActive);
    }

    #endregion

    #region Activity Management

    public void ScheduleActivity(Activity activity)
    {
        if (activity == null)
            throw new ArgumentNullException(nameof(activity));

        _activities.Add(activity);
        RecordActivity();

        AddDomainEvent(new ActivityScheduledEvent(
            TenantId, 
            activity.Id, 
            activity.Type, 
            activity.Subject, 
            activity.ScheduledAt, 
            activity.CreatedBy, 
            activity.AssignedTo));
    }

    public void CompleteActivity(Guid activityId, string completedBy, string? outcome = null, string? nextAction = null)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == activityId);
        if (activity == null)
            throw new ArgumentException("Activity not found", nameof(activityId));

        activity.Complete(outcome, nextAction);
        RecordActivity();

        AddDomainEvent(new ActivityCompletedEvent(
            TenantId, 
            activityId, 
            activity.Type, 
            completedBy, 
            outcome, 
            nextAction));
    }

    public IEnumerable<Activity> GetOverdueActivities()
    {
        return _activities.Where(a => a.IsOverdue);
    }

    public IEnumerable<Activity> GetUpcomingActivities(int days = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(days);
        return _activities.Where(a => a.Status == ActivityStatus.Scheduled && 
                                    a.ScheduledAt <= cutoff && 
                                    a.ScheduledAt >= DateTime.UtcNow)
                         .OrderBy(a => a.ScheduledAt);
    }

    #endregion

    #region Communication Management

    public void AddCommunication(Communication communication)
    {
        if (communication == null)
            throw new ArgumentNullException(nameof(communication));

        _communications.Add(communication);
        RecordActivity();
    }

    public IEnumerable<Communication> GetRecentCommunications(int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        return _communications.Where(c => c.CommunicatedAt >= cutoff)
                            .OrderByDescending(c => c.CommunicatedAt);
    }

    #endregion

    #region Notes and Documents

    public void AddNote(Note note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note));

        _notes.Add(note);
        RecordActivity();
    }

    public void AddDocument(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        _documents.Add(document);
        RecordActivity();
    }

    public IEnumerable<Document> GetActiveDocuments()
    {
        return _documents.Where(d => d.IsActive);
    }

    #endregion

    #region Customer Scoring

    public void UpdateScore(int newScore, string updatedBy)
    {
        CustomerScore = new LeadScore(newScore);
        UpdateModificationInfo(updatedBy);
    }

    public void IncreaseScore(int points, string updatedBy)
    {
        CustomerScore = CustomerScore.Increase(points);
        UpdateModificationInfo(updatedBy);
    }

    public void DecreaseScore(int points, string updatedBy)
    {
        CustomerScore = CustomerScore.Decrease(points);
        UpdateModificationInfo(updatedBy);
    }

    public bool IsHighValue => CustomerScore.IsHot || 
                              (AnnualRevenue?.Amount >= 100000) || 
                              Tier >= CustomerTier.Premium;

    #endregion

    #region Business Logic Helpers

    private void UpdateModificationInfo(string modifiedBy)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    private void RecordActivity()
    {
        LastActivityDate = DateTime.UtcNow;
    }

    public TimeSpan? TimeSinceLastActivity => LastActivityDate.HasValue ? 
        DateTime.UtcNow - LastActivityDate.Value : null;

    public bool IsStale => TimeSinceLastActivity.HasValue && TimeSinceLastActivity.Value.TotalDays > 90;

    public decimal GetLifetimeValue()
    {
        if (!MonthlyRecurringRevenue.HasValue || !SubscriptionStartDate.HasValue)
            return 0;

        var monthsActive = SubscriptionEndDate.HasValue ? 
            (decimal)(SubscriptionEndDate.Value - SubscriptionStartDate.Value).TotalDays / 30 :
            (decimal)(DateTime.UtcNow - SubscriptionStartDate.Value).TotalDays / 30;

        return MonthlyRecurringRevenue.Amount * Math.Max(0, monthsActive);
    }

    #endregion
}
