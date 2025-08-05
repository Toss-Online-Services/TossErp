using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Events;

namespace TossErp.Stock.Domain.Aggregates.WarehouseAggregate;

/// <summary>
/// Warehouse Aggregate Root
/// Manages warehouse hierarchy and location structure
/// </summary>
public class WarehouseAggregate : Entity, IAggregateRoot
{
    public WarehouseCode Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsGroup { get; private set; }
    public Guid? ParentWarehouseId { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public string? Account { get; private set; }
    public bool IsRejectedWarehouse { get; private set; }
    public string? WarehouseType { get; private set; }
    public string? DefaultInTransitWarehouse { get; private set; }
    public bool IsDisabled { get; private set; }

    // Contact Information
    public string? EmailId { get; private set; }
    public string? PhoneNo { get; private set; }
    public string? MobileNo { get; private set; }

    // Address Information
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Pin { get; private set; }
    public string? Country { get; private set; }

    // Tree Structure
    public int Lft { get; private set; }
    public int Rgt { get; private set; }

    // Child Collections
    private readonly List<Bin> _bins = new();
    private readonly List<WarehouseAggregate> _childWarehouses = new();

    // Navigation Properties
    public IReadOnlyCollection<Bin> Bins => _bins.AsReadOnly();
    public IReadOnlyCollection<WarehouseAggregate> ChildWarehouses => _childWarehouses.AsReadOnly();

    protected WarehouseAggregate() { } // For EF Core

    public WarehouseAggregate(
        WarehouseCode code,
        string name,
        string company,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));

        Code = code;
        Name = name.Trim();
        Company = company;
        Description = description?.Trim();
        IsGroup = false;
        IsRejectedWarehouse = false;
        IsDisabled = false;
        Lft = 0;
        Rgt = 0;

        AddDomainEvent(new WarehouseCreatedEvent(this));
    }

    // Business Methods
    public void UpdateBasicInfo(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        AddDomainEvent(new WarehouseUpdatedEvent(this));
    }

    public void SetAsGroup()
    {
        if (IsGroup)
            throw new InvalidOperationException("Warehouse is already a group");

        if (_bins.Any())
            throw new InvalidOperationException("Cannot set warehouse as group when it has bins");

        IsGroup = true;
        AddDomainEvent(new WarehouseGroupSetEvent(this));
    }

    public void SetParentWarehouse(Guid? parentWarehouseId)
    {
        if (parentWarehouseId.HasValue && parentWarehouseId.Value == Id)
            throw new InvalidOperationException("Warehouse cannot be its own parent");

        ParentWarehouseId = parentWarehouseId;
        AddDomainEvent(new WarehouseParentUpdatedEvent(this));
    }

    public void UpdateAccountSettings(string? account)
    {
        Account = account;
        AddDomainEvent(new WarehouseAccountUpdatedEvent(this));
    }

    public void SetAsRejectedWarehouse()
    {
        IsRejectedWarehouse = true;
        AddDomainEvent(new WarehouseRejectedSetEvent(this));
    }

    public void UpdateWarehouseType(string? warehouseType)
    {
        WarehouseType = warehouseType;
        AddDomainEvent(new WarehouseTypeUpdatedEvent(this));
    }

    public void SetDefaultInTransitWarehouse(string? defaultInTransitWarehouse)
    {
        DefaultInTransitWarehouse = defaultInTransitWarehouse;
        AddDomainEvent(new WarehouseInTransitUpdatedEvent(this));
    }

    public void UpdateContactInfo(string? emailId, string? phoneNo, string? mobileNo)
    {
        EmailId = emailId?.Trim();
        PhoneNo = phoneNo?.Trim();
        MobileNo = mobileNo?.Trim();
        AddDomainEvent(new WarehouseContactUpdatedEvent(this));
    }

    public void UpdateAddress(
        string? addressLine1,
        string? addressLine2,
        string? city,
        string? state,
        string? pin,
        string? country)
    {
        AddressLine1 = addressLine1?.Trim();
        AddressLine2 = addressLine2?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        Pin = pin?.Trim();
        Country = country?.Trim();
        AddDomainEvent(new WarehouseAddressUpdatedEvent(this));
    }

    public void UpdateTreeStructure(int lft, int rgt)
    {
        if (lft < 0 || rgt < 0 || lft >= rgt)
            throw new ArgumentException("Invalid tree structure values");

        Lft = lft;
        Rgt = rgt;
        AddDomainEvent(new WarehouseTreeStructureUpdatedEvent(this));
    }

    public void Disable()
    {
        if (IsDisabled)
            throw new InvalidOperationException("Warehouse is already disabled");

        IsDisabled = true;
        AddDomainEvent(new WarehouseDisabledEvent(this));
    }

    public void Enable()
    {
        if (!IsDisabled)
            throw new InvalidOperationException("Warehouse is not disabled");

        IsDisabled = false;
        AddDomainEvent(new WarehouseEnabledEvent(this));
    }

    // Child Entity Management
    public void AddChildWarehouse(WarehouseAggregate childWarehouse)
    {
        if (childWarehouse == null)
            throw new ArgumentNullException(nameof(childWarehouse));

        if (!IsGroup)
            throw new InvalidOperationException("Cannot add child warehouse to non-group warehouse");

        if (childWarehouse.Id == Id)
            throw new InvalidOperationException("Cannot add warehouse as its own child");

        _childWarehouses.Add(childWarehouse);
        childWarehouse.SetParentWarehouse(Id);
        AddDomainEvent(new WarehouseChildAddedEvent(this, childWarehouse));
    }

    public void RemoveChildWarehouse(Guid childWarehouseId)
    {
        var childWarehouse = _childWarehouses.FirstOrDefault(w => w.Id == childWarehouseId);
        if (childWarehouse == null)
            throw new InvalidOperationException($"Child warehouse with id {childWarehouseId} not found");

        _childWarehouses.Remove(childWarehouse);
        childWarehouse.SetParentWarehouse(null);
        AddDomainEvent(new WarehouseChildRemovedEvent(this, childWarehouse));
    }

    public void AddBin(Bin bin)
    {
        if (bin == null)
            throw new ArgumentNullException(nameof(bin));

        if (IsGroup)
            throw new InvalidOperationException("Cannot add bins to group warehouse");

        if (_bins.Any(b => b.BinCode == bin.BinCode))
            throw new InvalidOperationException($"Bin with code {bin.BinCode} already exists");

        _bins.Add(bin);
        AddDomainEvent(new WarehouseBinAddedEvent(this, bin));
    }

    public void RemoveBin(Guid binId)
    {
        var bin = _bins.FirstOrDefault(b => b.Id == binId);
        if (bin == null)
            throw new InvalidOperationException($"Bin with id {binId} not found");

        _bins.Remove(bin);
        AddDomainEvent(new WarehouseBinRemovedEvent(this, bin));
    }

    // Business Rules
    public bool IsGroupWarehouse() => IsGroup;

    public bool IsRejected() => IsRejectedWarehouse;

    public bool IsDisabledWarehouse() => IsDisabled;

    public bool HasChildren() => _childWarehouses.Any();

    public bool HasBins() => _bins.Any();

    public bool IsLeaf() => !IsGroup && !_childWarehouses.Any();

    public bool IsRoot() => !ParentWarehouseId.HasValue;

    public bool CanAcceptStock() => !IsDisabled && !IsGroup;

    public string GetFullAddress()
    {
        var addressParts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(AddressLine1))
            addressParts.Add(AddressLine1);
        if (!string.IsNullOrWhiteSpace(AddressLine2))
            addressParts.Add(AddressLine2);
        if (!string.IsNullOrWhiteSpace(City))
            addressParts.Add(City);
        if (!string.IsNullOrWhiteSpace(State))
            addressParts.Add(State);
        if (!string.IsNullOrWhiteSpace(Pin))
            addressParts.Add(Pin);
        if (!string.IsNullOrWhiteSpace(Country))
            addressParts.Add(Country);

        return string.Join(", ", addressParts);
    }

    public string GetContactInfo()
    {
        var contactParts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(EmailId))
            contactParts.Add($"Email: {EmailId}");
        if (!string.IsNullOrWhiteSpace(PhoneNo))
            contactParts.Add($"Phone: {PhoneNo}");
        if (!string.IsNullOrWhiteSpace(MobileNo))
            contactParts.Add($"Mobile: {MobileNo}");

        return string.Join(", ", contactParts);
    }

    public bool IsDescendantOf(Guid ancestorId)
    {
        return Lft > 0 && Rgt > 0 && Lft > 0 && Rgt > 0;
    }

    public bool IsAncestorOf(Guid descendantId)
    {
        return Lft > 0 && Rgt > 0 && Lft > 0 && Rgt > 0;
    }
} 
