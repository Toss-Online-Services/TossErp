using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Domain.Events;

/// <summary>
/// Supplier created event
/// </summary>
public class SupplierCreatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string Name { get; }
    public string Code { get; }
    public string TenantId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierCreatedEvent(Guid supplierId, string name, string code, string tenantId)
    {
        SupplierId = supplierId;
        Name = name;
        Code = code;
        TenantId = tenantId;
    }
}

/// <summary>
/// Supplier contact info updated event
/// </summary>
public class SupplierContactInfoUpdatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string? ContactPerson { get; }
    public string? Email { get; }
    public string? Phone { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierContactInfoUpdatedEvent(Guid supplierId, string? contactPerson, string? email, string? phone)
    {
        SupplierId = supplierId;
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
    }
}

/// <summary>
/// Supplier business info updated event
/// </summary>
public class SupplierBusinessInfoUpdatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string? TaxNumber { get; }
    public string? RegistrationNumber { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierBusinessInfoUpdatedEvent(Guid supplierId, string? taxNumber, string? registrationNumber)
    {
        SupplierId = supplierId;
        TaxNumber = taxNumber;
        RegistrationNumber = registrationNumber;
    }
}

/// <summary>
/// Supplier financial info updated event
/// </summary>
public class SupplierFinancialInfoUpdatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public decimal? CreditLimit { get; }
    public int? PaymentTermsDays { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierFinancialInfoUpdatedEvent(Guid supplierId, decimal? creditLimit, int? paymentTermsDays)
    {
        SupplierId = supplierId;
        CreditLimit = creditLimit;
        PaymentTermsDays = paymentTermsDays;
    }
}

/// <summary>
/// Supplier operational info updated event
/// </summary>
public class SupplierOperationalInfoUpdatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public decimal? LeadTimeDays { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierOperationalInfoUpdatedEvent(Guid supplierId, decimal? leadTimeDays)
    {
        SupplierId = supplierId;
        LeadTimeDays = leadTimeDays;
    }
}

/// <summary>
/// Supplier activated event
/// </summary>
public class SupplierActivatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string ActivatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierActivatedEvent(Guid supplierId, string activatedBy)
    {
        SupplierId = supplierId;
        ActivatedBy = activatedBy;
    }
}

/// <summary>
/// Supplier deactivated event
/// </summary>
public class SupplierDeactivatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string Reason { get; }
    public string DeactivatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierDeactivatedEvent(Guid supplierId, string reason, string deactivatedBy)
    {
        SupplierId = supplierId;
        Reason = reason;
        DeactivatedBy = deactivatedBy;
    }
}

/// <summary>
/// Supplier put on hold event
/// </summary>
public class SupplierPutOnHoldEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string Reason { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierPutOnHoldEvent(Guid supplierId, string reason, string updatedBy)
    {
        SupplierId = supplierId;
        Reason = reason;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Supplier blacklisted event
/// </summary>
public class SupplierBlacklistedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string Reason { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierBlacklistedEvent(Guid supplierId, string reason, string updatedBy)
    {
        SupplierId = supplierId;
        Reason = reason;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Supplier notes updated event
/// </summary>
public class SupplierNotesUpdatedEvent : IDomainEvent
{
    public Guid SupplierId { get; }
    public string Notes { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SupplierNotesUpdatedEvent(Guid supplierId, string notes, string updatedBy)
    {
        SupplierId = supplierId;
        Notes = notes;
        UpdatedBy = updatedBy;
    }
}
