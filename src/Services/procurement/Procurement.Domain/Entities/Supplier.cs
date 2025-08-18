using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Domain.Entities;

/// <summary>
/// Supplier entity representing a vendor/supplier
/// </summary>
public class Supplier : Entity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public SupplierStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public int? PaymentTermsDays { get; private set; }
    public decimal? LeadTimeDays { get; private set; }

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Supplier() : base() { } // For EF Core

    public Supplier(Guid id, string name, string code, string tenantId) : base(id, tenantId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Status = SupplierStatus.PendingApproval;

        AddDomainEvent(new SupplierCreatedEvent(Id, Name, Code, tenantId));
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    public static Supplier Create(string name, string code, string tenantId)
    {
        return new Supplier(Guid.NewGuid(), name, code, tenantId);
    }

    /// <summary>
    /// Update supplier contact information
    /// </summary>
    public void UpdateContactInfo(string? contactPerson, string? email, string? phone, string? address, 
        string? city, string? postalCode, string? country, string updatedBy)
    {
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Address = address;
        City = city;
        PostalCode = postalCode;
        Country = country;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierContactInfoUpdatedEvent(Id, contactPerson, email, phone));
    }

    /// <summary>
    /// Update supplier business information
    /// </summary>
    public void UpdateBusinessInfo(string? taxNumber, string? registrationNumber, string updatedBy)
    {
        TaxNumber = taxNumber;
        RegistrationNumber = registrationNumber;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierBusinessInfoUpdatedEvent(Id, taxNumber, registrationNumber));
    }

    /// <summary>
    /// Update supplier financial information
    /// </summary>
    public void UpdateFinancialInfo(decimal? creditLimit, int? paymentTermsDays, string updatedBy)
    {
        CreditLimit = creditLimit;
        PaymentTermsDays = paymentTermsDays;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierFinancialInfoUpdatedEvent(Id, creditLimit, paymentTermsDays));
    }

    /// <summary>
    /// Update supplier operational information
    /// </summary>
    public void UpdateOperationalInfo(decimal? leadTimeDays, string updatedBy)
    {
        LeadTimeDays = leadTimeDays;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierOperationalInfoUpdatedEvent(Id, leadTimeDays));
    }

    /// <summary>
    /// Activate the supplier
    /// </summary>
    public void Activate(string activatedBy)
    {
        if (Status == SupplierStatus.Active)
            throw new InvalidOperationException("Supplier is already active");

        Status = SupplierStatus.Active;
        MarkAsUpdated(activatedBy);

        AddDomainEvent(new SupplierActivatedEvent(Id, activatedBy));
    }

    /// <summary>
    /// Deactivate the supplier
    /// </summary>
    public void Deactivate(string reason, string deactivatedBy)
    {
        if (Status == SupplierStatus.Inactive)
            throw new InvalidOperationException("Supplier is already inactive");

        Status = SupplierStatus.Inactive;
        Notes = reason;
        MarkAsUpdated(deactivatedBy);

        AddDomainEvent(new SupplierDeactivatedEvent(Id, reason, deactivatedBy));
    }

    /// <summary>
    /// Put supplier on hold
    /// </summary>
    public void PutOnHold(string reason, string updatedBy)
    {
        if (Status == SupplierStatus.OnHold)
            throw new InvalidOperationException("Supplier is already on hold");

        Status = SupplierStatus.OnHold;
        Notes = reason;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierPutOnHoldEvent(Id, reason, updatedBy));
    }

    /// <summary>
    /// Blacklist the supplier
    /// </summary>
    public void Blacklist(string reason, string updatedBy)
    {
        if (Status == SupplierStatus.Blacklisted)
            throw new InvalidOperationException("Supplier is already blacklisted");

        Status = SupplierStatus.Blacklisted;
        Notes = reason;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierBlacklistedEvent(Id, reason, updatedBy));
    }

    /// <summary>
    /// Add notes to the supplier
    /// </summary>
    public void AddNotes(string notes, string updatedBy)
    {
        Notes = notes;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SupplierNotesUpdatedEvent(Id, notes, updatedBy));
    }

    /// <summary>
    /// Add domain event
    /// </summary>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear domain events (typically called after publishing)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
