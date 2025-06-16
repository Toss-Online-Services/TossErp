using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.Models;

namespace POS.Domain.AggregatesModel.StoreAggregate;

public class Store : AggregateRoot
{
    public new int Id { get; set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public string? TaxId { get; private set; }
    public string? Notes { get; private set; }
    public Address Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public string? Logo { get; private set; }
    public string? Currency { get; private set; }
    public string? TimeZone { get; private set; }
    public string Status { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSynced { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Store()
    {
        Name = string.Empty;
        Address = new Address();
        Status = "Active";
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Store(string name, Address address, string? description = null, string? code = null, string? taxId = null,
        string? notes = null, string? phone = null, string? email = null, string? website = null,
        string? logo = null, string? currency = null, string? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (address == null)
            throw new DomainException("Address cannot be null");

        Name = name;
        Address = address;
        Description = description;
        Code = code;
        TaxId = taxId;
        Notes = notes;
        Phone = phone;
        Email = email;
        Website = website;
        Logo = logo;
        Currency = currency;
        TimeZone = timeZone;
        Status = "Active";
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, Address address, string? description = null, string? code = null,
        string? taxId = null, string? notes = null, string? phone = null, string? email = null,
        string? website = null, string? logo = null, string? currency = null, string? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (address == null)
            throw new DomainException("Address cannot be null");

        Name = name;
        Address = address;
        Description = description;
        Code = code;
        TaxId = taxId;
        Notes = notes;
        Phone = phone;
        Email = email;
        Website = website;
        Logo = logo;
        Currency = currency;
        TimeZone = timeZone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Store is already inactive");

        IsActive = false;
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Store is already active");

        IsActive = true;
        Status = "Active";
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSynced()
    {
        IsSynced = true;
        SyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOffline()
    {
        IsSynced = false;
        SyncedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
} 
