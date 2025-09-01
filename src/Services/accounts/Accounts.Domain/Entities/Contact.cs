using TossErp.Shared.SeedWork;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Contact entity for managing contact information
/// </summary>
[Table("Contacts")]
public class Contact : Entity
{
    public override Guid Id { get; protected set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string FirstName { get; private set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; private set; } = string.Empty;

    [StringLength(100)]
    public string? Email { get; private set; }

    [StringLength(20)]
    public string? Phone { get; private set; }

    [StringLength(20)]
    public string? MobileNumber { get; private set; }

    [StringLength(100)]
    public string? JobTitle { get; private set; }

    [StringLength(100)]
    public string? Department { get; private set; }

    public bool IsPrimary { get; private set; } = false;

    public bool IsActive { get; private set; } = true;

    [StringLength(500)]
    public string? Notes { get; private set; }

    // For customer contacts
    public Guid? CustomerId { get; private set; }
    public virtual Customer? Customer { get; private set; }

    // For vendor contacts
    public Guid? VendorId { get; private set; }
    public virtual Vendor? Vendor { get; private set; }

    private Contact() : base() { }

    public Contact(
        Guid id,
        string tenantId,
        string firstName,
        string lastName,
        string? email = null,
        string? phone = null,
        string? mobileNumber = null,
        string? jobTitle = null,
        string? department = null,
        bool isPrimary = false,
        string? notes = null,
        Guid? customerId = null,
        Guid? vendorId = null,
        string? createdBy = null) : base(id, tenantId)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email;
        Phone = phone;
        MobileNumber = mobileNumber;
        JobTitle = jobTitle;
        Department = department;
        IsPrimary = isPrimary;
        Notes = notes;
        CustomerId = customerId;
        VendorId = vendorId;
        CreatedBy = createdBy;
    }

    public static Contact Create(
        string tenantId,
        string firstName,
        string lastName,
        string? email = null,
        string? phone = null,
        string? mobileNumber = null,
        string? jobTitle = null,
        string? department = null,
        bool isPrimary = false,
        string? notes = null,
        Guid? customerId = null,
        Guid? vendorId = null,
        string? createdBy = null)
    {
        return new Contact(
            Guid.NewGuid(),
            tenantId,
            firstName,
            lastName,
            email,
            phone,
            mobileNumber,
            jobTitle,
            department,
            isPrimary,
            notes,
            customerId,
            vendorId,
            createdBy);
    }

    public void UpdateContactInfo(
        string firstName,
        string lastName,
        string? email = null,
        string? phone = null,
        string? mobileNumber = null,
        string? jobTitle = null,
        string? department = null,
        string? notes = null,
        string? updatedBy = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email;
        Phone = phone;
        MobileNumber = mobileNumber;
        JobTitle = jobTitle;
        Department = department;
        Notes = notes;
        MarkAsUpdated(updatedBy);
    }

    public void SetAsPrimary(string? updatedBy = null)
    {
        IsPrimary = true;
        MarkAsUpdated(updatedBy);
    }

    public void UnsetAsPrimary(string? updatedBy = null)
    {
        IsPrimary = false;
        MarkAsUpdated(updatedBy);
    }

    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        MarkAsUpdated(updatedBy);
    }

    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        MarkAsUpdated(updatedBy);
    }
}
