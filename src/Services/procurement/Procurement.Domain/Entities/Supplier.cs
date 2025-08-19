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
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public SupplierStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public int? PaymentTermsDays { get; private set; }
    public string? BankName { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public string? BankRoutingNumber { get; private set; }
    public string? PaymentTerms { get; private set; }
    public decimal? LeadTimeDays { get; private set; }

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Supplier() : base() { } // For EF Core

    public Supplier(Guid id, string name, string code, string tenantId) : base(id, tenantId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Status = SupplierStatus.Active; // default to Active for compatibility with tests

        AddDomainEvent(new SupplierCreatedEvent(Id, Name, Code, tenantId));
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    public static Supplier Create(string code, string name, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code cannot be empty", nameof(code));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
        return new Supplier(Guid.NewGuid(), name, code, tenantId);
    }

    /// <summary>
    /// Update supplier contact information
    /// </summary>
    public void UpdateContactInfo(string? contactPerson, string? email, string? phone)
    {
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        AddDomainEvent(new SupplierContactInfoUpdatedEvent(Id, contactPerson, email, phone));
    }

    /// <summary>
    /// Update supplier business information
    /// </summary>
    public void UpdateBusinessInfo(string? address, string? city, string? state, string? postalCode, string? country, string? taxNumber)
    {
        Address = address;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        TaxNumber = taxNumber;
        AddDomainEvent(new SupplierBusinessInfoUpdatedEvent(Id, taxNumber, RegistrationNumber));
    }

    /// <summary>
    /// Update supplier financial information
    /// </summary>
    public void UpdateFinancialInfo(string? bankName, string? bankAccountNumber, string? bankRoutingNumber, string? paymentTerms)
    {
        BankName = bankName;
        BankAccountNumber = bankAccountNumber;
        BankRoutingNumber = bankRoutingNumber;
        PaymentTerms = paymentTerms;
        AddDomainEvent(new SupplierFinancialInfoUpdatedEvent(Id, CreditLimit, PaymentTermsDays));
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
    public void Activate()
    {
        if (Status == SupplierStatus.Active)
            throw new InvalidOperationException("Supplier is already active");

        Status = SupplierStatus.Active;
        AddDomainEvent(new SupplierActivatedEvent(Id, "system"));
    }

    /// <summary>
    /// Deactivate the supplier
    /// </summary>
    public void Deactivate()
    {
        if (Status == SupplierStatus.Inactive)
            throw new InvalidOperationException("Supplier is already inactive");

        Status = SupplierStatus.Inactive;
        AddDomainEvent(new SupplierDeactivatedEvent(Id, null!, "system"));
    }

    /// <summary>
    /// Put supplier on hold
    /// </summary>
    public void Hold()
    {
        if (Status == SupplierStatus.OnHold)
            throw new InvalidOperationException("Supplier is already on hold");

        Status = SupplierStatus.OnHold;
        AddDomainEvent(new SupplierPutOnHoldEvent(Id, null!, "system"));
    }

    /// <summary>
    /// Blacklist the supplier
    /// </summary>
    public void Blacklist(string reason)
    {
        if (Status == SupplierStatus.Blacklisted)
            throw new InvalidOperationException("Supplier is already blacklisted");

        Status = SupplierStatus.Blacklisted;
        Notes = reason;
        AddDomainEvent(new SupplierBlacklistedEvent(Id, reason, "system"));
    }

    /// <summary>
    /// Add notes to the supplier
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes} {notes}";
        AddDomainEvent(new SupplierNotesUpdatedEvent(Id, notes, "system"));
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
