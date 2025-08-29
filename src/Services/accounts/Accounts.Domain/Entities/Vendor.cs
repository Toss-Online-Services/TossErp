using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Vendor entity for supplier management with township SMME features
/// </summary>
public class Vendor : AggregateRoot
{
    private readonly List<VendorContact> _contacts = [];

    public Vendor(
        Guid id,
        string tenantId,
        string vendorNumber,
        string name,
        VendorType vendorType,
        string createdBy) : base(id, tenantId)
    {
        VendorNumber = vendorNumber?.Trim() ?? throw new ArgumentException("Vendor number cannot be empty");
        Name = name?.Trim() ?? throw new ArgumentException("Vendor name cannot be empty");
        VendorType = vendorType;
        Status = VendorStatus.Active;
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
        Currency = "ZAR"; // Default to South African Rand for township vendors
    }

    // Basic vendor information
    public string VendorNumber { get; private set; }
    public string Name { get; private set; }
    public VendorType VendorType { get; private set; }
    public VendorStatus Status { get; private set; }
    
    // Contact information
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    
    // Financial information - Support for township SMMEs
    public string Currency { get; private set; }
    public Money CreditLimit { get; private set; } = Money.Zero(CurrencyCode.ZAR);
    public PaymentTerms PaymentTerms { get; private set; } = PaymentTerms.Net30;
    public Money CurrentBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);
    
    // Township SMME specific features
    public bool IsLocalSupplier { get; private set; } = true; // Support local township suppliers
    public bool IsCommunityCooperative { get; private set; }
    public bool IsWomenOwned { get; private set; }
    public bool IsYouthOwned { get; private set; }
    public bool IsBBBEECompliant { get; private set; }
    
    // Address information
    public string? PhysicalAddress { get; private set; }
    public string? PostalAddress { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; } = "South Africa";
    
    // Tax information
    public string? TaxNumber { get; private set; }
    public string? VATNumber { get; private set; }
    public bool IsVATRegistered { get; private set; }
    
    // Banking information
    public string? BankName { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public string? BankBranch { get; private set; }
    public string? BankBranchCode { get; private set; }
    
    // Township business features
    public bool AcceptsMobilePayments { get; private set; } = true; // Support for mobile money
    public bool OffersStoreCredit { get; private set; }
    public bool SupportsGroupPurchasing { get; private set; } = true; // Community purchasing
    
    // Audit fields - UpdatedAt and UpdatedBy already in base AggregateRoot
    public DateTime ModifiedAt { get; private set; }
    public string ModifiedBy { get; private set; }
    
    // Notes and additional information
    public string? Notes { get; private set; }
    public string? InternalReference { get; private set; }
    
    public IReadOnlyCollection<VendorContact> Contacts => _contacts.AsReadOnly();

    // Business methods
    public void UpdateBasicInfo(
        string name,
        VendorType vendorType,
        string? email,
        string? phone,
        string? website,
        string modifiedBy)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Vendor name cannot be empty");
        VendorType = vendorType;
        Email = email?.Trim();
        Phone = phone?.Trim();
        Website = website?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateFinancialInfo(
        Money creditLimit,
        PaymentTerms paymentTerms,
        string modifiedBy)
    {
        CreditLimit = creditLimit ?? throw new ArgumentNullException(nameof(creditLimit));
        PaymentTerms = paymentTerms;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateTownshipFeatures(
        bool isLocalSupplier,
        bool isCommunityCooperative,
        bool isWomenOwned,
        bool isYouthOwned,
        bool isBBBEECompliant,
        bool acceptsMobilePayments,
        bool offersStoreCredit,
        bool supportsGroupPurchasing,
        string modifiedBy)
    {
        IsLocalSupplier = isLocalSupplier;
        IsCommunityCooperative = isCommunityCooperative;
        IsWomenOwned = isWomenOwned;
        IsYouthOwned = isYouthOwned;
        IsBBBEECompliant = isBBBEECompliant;
        AcceptsMobilePayments = acceptsMobilePayments;
        OffersStoreCredit = offersStoreCredit;
        SupportsGroupPurchasing = supportsGroupPurchasing;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateAddress(
        string? physicalAddress,
        string? postalAddress,
        string? city,
        string? province,
        string? postalCode,
        string? country,
        string modifiedBy)
    {
        PhysicalAddress = physicalAddress?.Trim();
        PostalAddress = postalAddress?.Trim();
        City = city?.Trim();
        Province = province?.Trim();
        PostalCode = postalCode?.Trim();
        Country = country?.Trim() ?? "South Africa";
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateTaxInfo(
        string? taxNumber,
        string? vatNumber,
        bool isVATRegistered,
        string modifiedBy)
    {
        TaxNumber = taxNumber?.Trim();
        VATNumber = vatNumber?.Trim();
        IsVATRegistered = isVATRegistered;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateBankingInfo(
        string? bankName,
        string? bankAccountNumber,
        string? bankBranch,
        string? bankBranchCode,
        string modifiedBy)
    {
        BankName = bankName?.Trim();
        BankAccountNumber = bankAccountNumber?.Trim();
        BankBranch = bankBranch?.Trim();
        BankBranchCode = bankBranchCode?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Activate(string modifiedBy)
    {
        if (Status == VendorStatus.Active)
            throw new InvalidOperationException("Vendor is already active");

        Status = VendorStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Deactivate(string reason, string modifiedBy)
    {
        if (Status == VendorStatus.Inactive)
            throw new InvalidOperationException("Vendor is already inactive");

        Status = VendorStatus.Inactive;
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}; Deactivated: {reason}";
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void AddContact(VendorContact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        // Check for duplicate contact types
        if (_contacts.Any(c => c.ContactType == contact.ContactType && c.ContactType != ContactType.Other))
            throw new InvalidOperationException($"A contact of type {contact.ContactType} already exists");

        _contacts.Add(contact);
        ModifiedAt = DateTime.UtcNow;
    }

    public void RemoveContact(Guid contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact != null)
        {
            _contacts.Remove(contact);
            ModifiedAt = DateTime.UtcNow;
        }
    }

    public void UpdateNotes(string? notes, string modifiedBy)
    {
        Notes = notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateBalance(Money amount, string reason, string modifiedBy)
    {
        CurrentBalance = amount ?? throw new ArgumentNullException(nameof(amount));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public bool IsOverCreditLimit(Money additionalAmount)
    {
        var totalAmount = CurrentBalance.Add(additionalAmount);
        return totalAmount.Amount > CreditLimit.Amount;
    }

    public Money AvailableCredit => CreditLimit.Subtract(CurrentBalance);
}

/// <summary>
/// Vendor contact information with township business context
/// </summary>
public class VendorContact : Entity
{
    public VendorContact(
        Guid id,
        string tenantId,
        Guid vendorId,
        ContactType contactType,
        string name,
        string createdBy) : base(id, tenantId)
    {
        VendorId = vendorId;
        ContactType = contactType;
        Name = name?.Trim() ?? throw new ArgumentException("Contact name cannot be empty");
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
    }

    public Guid VendorId { get; private set; }
    public ContactType ContactType { get; private set; }
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }
    public string? Position { get; private set; }
    public string? Department { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Township business features
    public bool CanReceiveSMS { get; private set; } = true; // For mobile notifications
    public bool CanReceiveWhatsApp { get; private set; } = true; // Popular in townships
    public string? WhatsAppNumber { get; private set; }
    public string? PreferredLanguage { get; private set; } = "English"; // Support local languages

    public DateTime ModifiedAt { get; private set; }
    public string ModifiedBy { get; private set; }

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
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
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
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetAsPrimary(string modifiedBy)
    {
        IsPrimary = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void RemoveAsPrimary(string modifiedBy)
    {
        IsPrimary = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        IsPrimary = false; // Cannot be primary if inactive
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }
}
