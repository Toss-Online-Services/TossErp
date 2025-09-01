using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Customer entity representing customers in the TOSS ERP system
/// Designed for South African township SMME context
/// </summary>
[Table("Customers")]
public class Customer : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }

    [Required]
    [StringLength(200)]
    public string Name { get; private set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; private set; }

    [StringLength(100)]
    public string? LastName { get; private set; }

    [StringLength(100)]
    public string? Email { get; private set; }

    [StringLength(20)]
    public string? Phone { get; private set; }

    [StringLength(20)]
    public string? MobileNumber { get; private set; }

    [StringLength(50)]
    public string? IdNumber { get; private set; }

    [StringLength(20)]
    public string? PassportNumber { get; private set; }

    public CustomerType CustomerType { get; private set; } = CustomerType.Individual;

    public CustomerStatus Status { get; private set; } = CustomerStatus.Active;

    [StringLength(50)]
    public string? CustomerCode { get; private set; }

    [StringLength(50)]
    public string? TaxId { get; private set; }

    public Money CreditLimit { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money CurrentBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public PaymentTerms PaymentTerms { get; private set; } = PaymentTerms.Net30;

    [StringLength(3)]
    public string Currency { get; private set; } = "ZAR";

    [StringLength(100)]
    public string? PrimaryLanguage { get; private set; }

    // Township/Rural SMME specific fields
    [StringLength(200)]
    public string? TownshipLocation { get; private set; }

    [StringLength(100)]
    public string? CommunityGroup { get; private set; }

    public bool IsStoreCreditCustomer { get; private set; } = false;

    public Money StoreCreditBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public DateTime? LastTransactionDate { get; private set; }

    [StringLength(100)]
    public string? PreferredPaymentMethod { get; private set; }

    // Loyalty program
    public int LoyaltyPoints { get; private set; } = 0;

    public DateTime? DateOfBirth { get; private set; }

    [StringLength(20)]
    public string? Gender { get; private set; }

    [StringLength(500)]
    public string? Notes { get; private set; }

    public bool IsActive { get; private set; } = true;    public override DateTime ModifiedAt { get; protected set; } = DateTime.UtcNow;

    [StringLength(100)]    public override string? ModifiedBy { get; protected set; }

    // Application layer compatibility properties
    [StringLength(100)]
    public string? TaxNumber { get; private set; }

    [StringLength(200)]
    public string? Website { get; private set; }

    [StringLength(3)]
    public string? PreferredCurrency { get; private set; }

    public DateTime? LastPaymentDate { get; private set; }

    public DateTime? LastModified => ModifiedAt;

    public string? LastModifiedBy => ModifiedBy;

    [StringLength(200)]
    public string? CompanyName { get; private set; }

    // Application layer compatibility property accessors
    public string? CustomerNumber => CustomerCode;
    public CustomerType Type => CustomerType;
    public string? PreferredLanguage => PrimaryLanguage;

    // Navigation properties for addresses
    public CustomerAddress? BillingAddress => _addresses.FirstOrDefault(a => a.Type == "Billing" || a.IsPrimary);
    public CustomerAddress? ShippingAddress => _addresses.FirstOrDefault(a => a.Type == "Shipping") ?? BillingAddress;

    // Navigation properties for contacts  
    public CustomerContact? PrimaryContact => _contacts.FirstOrDefault(c => c.IsPrimary);
    public IReadOnlyList<CustomerContact> AdditionalContacts => _contacts.Where(c => !c.IsPrimary).ToList().AsReadOnly();

    // Navigation collection for contacts
    private readonly List<CustomerContact> _contacts = new();
    public IReadOnlyList<CustomerContact> Contacts => _contacts.AsReadOnly();

    // Navigation properties
    private readonly List<CustomerAddress> _addresses = new();
    public IReadOnlyList<CustomerAddress> Addresses => _addresses.AsReadOnly();

    private readonly List<Invoice> _invoices = new();
    public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    private Customer() : base() { } // EF Core

    public Customer(
        Guid id,
        string tenantId,
        string name,
        string createdBy,
        CustomerType customerType = CustomerType.Individual,
        string? email = null,
        string? phone = null,
        string? mobileNumber = null,
        string? customerCode = null)
    {
        Id = id;
        Name = name?.Trim() ?? throw new ArgumentException("Customer name cannot be empty");
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CustomerType = customerType;
        Email = email?.Trim();
        Phone = phone?.Trim();
        MobileNumber = mobileNumber?.Trim();
        CustomerCode = customerCode?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
    }

    public static Customer Create(
        string tenantId,
        string name,
        string createdBy,
        CustomerType customerType = CustomerType.Individual,
        string? email = null,
        string? phone = null,
        string? mobileNumber = null,
        string? customerCode = null)
    {
        return new Customer(Guid.NewGuid(), tenantId, name, createdBy, customerType, email, phone, mobileNumber, customerCode);
    }

    public void UpdateBasicInfo(
        string name,
        string? firstName,
        string? lastName,
        string? email,
        string? phone,
        string? mobileNumber,
        string modifiedBy)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Customer name cannot be empty");
        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
        MobileNumber = mobileNumber?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateTownshipInfo(
        string? townshipLocation,
        string? communityGroup,
        string? primaryLanguage,
        string modifiedBy)
    {
        TownshipLocation = townshipLocation?.Trim();
        CommunityGroup = communityGroup?.Trim();
        PrimaryLanguage = primaryLanguage?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetCreditLimit(Money creditLimit, string modifiedBy)
    {
        CreditLimit = creditLimit ?? throw new ArgumentNullException(nameof(creditLimit));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateBalance(Money amount, string modifiedBy)
    {
        CurrentBalance = amount ?? throw new ArgumentNullException(nameof(amount));
        LastTransactionDate = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void AddToStoreCredit(Money amount, string modifiedBy)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Store credit amount must be positive");

        IsStoreCreditCustomer = true;
        StoreCreditBalance = StoreCreditBalance.Add(amount);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UseStoreCredit(Money amount, string modifiedBy)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Store credit usage amount must be positive");

        if (StoreCreditBalance.Amount < amount.Amount)
            throw new InvalidOperationException("Insufficient store credit balance");

        StoreCreditBalance = StoreCreditBalance.Subtract(amount);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void AddLoyaltyPoints(int points, string modifiedBy)
    {
        if (points < 0)
            throw new ArgumentException("Loyalty points cannot be negative");

        LoyaltyPoints += points;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void RedeemLoyaltyPoints(int points, string modifiedBy)
    {
        if (points < 0)
            throw new ArgumentException("Points to redeem cannot be negative");

        if (LoyaltyPoints < points)
            throw new InvalidOperationException("Insufficient loyalty points");

        LoyaltyPoints -= points;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void AddAddress(CustomerAddress address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        _addresses.Add(address);
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        Status = CustomerStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        Status = CustomerStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public bool HasAvailableCredit(Money amount)
    {
        var availableCredit = CreditLimit.Subtract(CurrentBalance);
        return availableCredit.Amount >= amount.Amount;
    }

    // Domain methods required by application layer
    public void UpdatePrimaryContact(CustomerContact contact, string modifiedBy)
    {
        if (contact == null) throw new ArgumentNullException(nameof(contact));
        
        // Set all contacts to non-primary first
        foreach (var existingContact in _contacts)
        {
            existingContact.RemoveAsPrimary(modifiedBy);
        }
        
        // Set the new primary contact
        contact.SetAsPrimary(modifiedBy);
        MarkAsUpdated(modifiedBy);
    }

    public void SetPreferredCurrency(string currencyCode, string modifiedBy)
    {
        PreferredCurrency = currencyCode?.Trim() ?? throw new ArgumentException("Currency code cannot be empty");
        MarkAsUpdated(modifiedBy);
    }

    public void SetPreferredLanguage(string language, string modifiedBy)
    {
        PrimaryLanguage = language?.Trim() ?? throw new ArgumentException("Language cannot be empty");
        MarkAsUpdated(modifiedBy);
    }

    public void SetPaymentTerms(PaymentTerms paymentTerms, string modifiedBy)
    {
        PaymentTerms = paymentTerms;
        MarkAsUpdated(modifiedBy);
    }

    public void ChangeStatus(CustomerStatus status, string modifiedBy)
    {
        Status = status;
        MarkAsUpdated(modifiedBy);
    }

    public void SetCompanyName(string companyName, string modifiedBy)
    {
        CompanyName = companyName?.Trim();
        MarkAsUpdated(modifiedBy);
    }
}

/// <summary>
/// Customer Address entity
/// </summary>
[Table("CustomerAddresses")]
public class CustomerAddress : Entity
{
    public override Guid Id { get; protected set; }
    public Guid CustomerId { get; private set; }

    [Required]
    [StringLength(20)]
    public string AddressType { get; private set; } = string.Empty; // Billing, Shipping, etc.

    [StringLength(500)]
    public string? AddressLine1 { get; private set; }

    [StringLength(500)]
    public string? AddressLine2 { get; private set; }

    [StringLength(100)]
    public string? City { get; private set; }

    [StringLength(100)]
    public string? State { get; private set; }

    [StringLength(20)]
    public string? PostalCode { get; private set; }

    [StringLength(100)]
    public string? Country { get; private set; } = "South Africa";

    // Township specific fields
    [StringLength(200)]
    public string? TownshipName { get; private set; }

    [StringLength(200)]
    public string? Ward { get; private set; }

    [StringLength(200)]
    public string? Suburb { get; private set; }

    [StringLength(100)]
    public string? Province { get; private set; }

    public bool IsPrimaryAddress { get; private set; } = false;

    public bool IsActive { get; private set; } = true;

    // Application layer compatibility properties
    public string Street => AddressLine1 ?? string.Empty;
    public string? Street2 => AddressLine2;
    public string? Type => AddressType; // Application layer compatibility
    public bool IsPrimary => IsPrimaryAddress; // Application layer compatibility

    // Navigation properties
    public Customer Customer { get; private set; } = null!;

    private CustomerAddress() : base() { } // EF Core

    public CustomerAddress(
        Guid id,
        string tenantId,
        Guid customerId,
        string addressType,
        string? addressLine1 = null,
        string? city = null,
        string? country = "South Africa")
    {
        CustomerId = customerId;
        AddressType = addressType?.Trim() ?? throw new ArgumentException("Address type cannot be empty");
        AddressLine1 = addressLine1?.Trim();
        City = city?.Trim();
        Country = country?.Trim();
    }

    public static CustomerAddress Create(
        string tenantId,
        Guid customerId,
        string addressType,
        string? addressLine1 = null,
        string? city = null,
        string? country = "South Africa")
    {
        return new CustomerAddress(Guid.NewGuid(), tenantId, customerId, addressType, addressLine1, city, country);
    }

    public void UpdateAddress(
        string? addressLine1,
        string? addressLine2,
        string? city,
        string? state,
        string? postalCode,
        string? country,
        string? townshipName = null,
        string? ward = null,
        string? suburb = null,
        string? province = null)
    {
        AddressLine1 = addressLine1?.Trim();
        AddressLine2 = addressLine2?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        PostalCode = postalCode?.Trim();
        Country = country?.Trim();
        TownshipName = townshipName?.Trim();
        Ward = ward?.Trim();
        Suburb = suburb?.Trim();
        Province = province?.Trim();
    }

    public void SetAsPrimary()
    {
        IsPrimaryAddress = true;
    }

    public void RemoveAsPrimary()
    {
        IsPrimaryAddress = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

/// <summary>
/// Customer contact information with township business context
/// </summary>
public class CustomerContact : Entity
{
    public override Guid Id { get; protected set; }
    public CustomerContact(
        Guid id,
        string tenantId,
        Guid customerId,
        ContactType contactType,
        string name,
        string createdBy)
    {
        CustomerId = customerId;
        ContactType = contactType;
        Name = name?.Trim() ?? throw new ArgumentException("Contact name cannot be empty");
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        MarkAsUpdated(createdBy);
    }

    public Guid CustomerId { get; private set; }
    public ContactType ContactType { get; private set; }
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }
    public string? Position { get; private set; }
    public string? Department { get; private set; }
    public string? Title { get; private set; } // Application layer compatibility
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Township business features
    public bool CanReceiveSMS { get; private set; } = true; // For mobile notifications
    public bool CanReceiveWhatsApp { get; private set; } = true; // Popular in townships
    public string? WhatsAppNumber { get; private set; }
    public string? PreferredLanguage { get; private set; } = "English"; // Support local languages

    public void UpdateContactInfo(
        string name,
        string? email,
        string? phone,
        string? mobile,
        string? position,
        string? department,
        string modifiedBy)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Contact name cannot be empty");
        Email = email?.Trim();
        Phone = phone?.Trim();
        Mobile = mobile?.Trim();
        Position = position?.Trim();
        Department = department?.Trim();
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }

    public void UpdateTownshipFeatures(
        bool canReceiveSMS,
        bool canReceiveWhatsApp,
        string? whatsAppNumber,
        string? preferredLanguage,
        string modifiedBy)
    {
        CanReceiveSMS = canReceiveSMS;
        CanReceiveWhatsApp = canReceiveWhatsApp;
        WhatsAppNumber = whatsAppNumber?.Trim();
        PreferredLanguage = preferredLanguage?.Trim() ?? "English";
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }

    public void SetAsPrimary(string modifiedBy)
    {
        IsPrimary = true;
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }

    public void RemoveAsPrimary(string modifiedBy)
    {
        IsPrimary = false;
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        IsPrimary = false; // Cannot be primary if inactive
        MarkAsUpdated(modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty"));
    }
}
