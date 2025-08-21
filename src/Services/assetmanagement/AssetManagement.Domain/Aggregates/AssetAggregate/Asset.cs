using TossErp.AssetManagement.Domain.SeedWork;
using TossErp.AssetManagement.Domain.ValueObjects;
using TossErp.AssetManagement.Domain.Enums;
using TossErp.AssetManagement.Domain.Aggregates.AssetAggregate.Events;

namespace TossErp.AssetManagement.Domain.Aggregates.AssetAggregate;

/// <summary>
/// Asset Aggregate - Central entity for asset management following DDD principles
/// Inspired by ERPNext Asset module but tailored for SaaS multi-tenant architecture
/// </summary>
public class Asset : Entity, IAggregateRoot
{
    // Core Identity
    public AssetTag AssetTag { get; private set; } = null!;
    public string AssetName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public AssetCategory Category { get; private set; }
    public AssetStatus Status { get; private set; }
    public AssetCondition Condition { get; private set; }

    // Physical Attributes
    public string? Model { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? SerialNumber { get; private set; }
    public AssetDimensions? Dimensions { get; private set; }
    public AssetSpecification? Specifications { get; private set; }

    // Location and Ownership
    public AssetLocation Location { get; private set; } = null!;
    public AssetOwnership OwnershipType { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public string? Department { get; private set; }

    // Financial Information
    public AssetFinancialInfo FinancialInfo { get; private set; } = null!;
    public DepreciationMethod DepreciationMethod { get; private set; }
    public int UsefulLifeYears { get; private set; }

    // Warranty and Support
    public AssetWarranty? Warranty { get; private set; }
    public string? SupportContact { get; private set; }

    // Audit and Tracking
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    public DateTime? DisposalDate { get; private set; }
    public string? DisposalReason { get; private set; }

    // Multi-tenant SaaS support
    public Guid TenantId { get; private set; }

    // Child Collections (following DDD aggregate boundaries)
    private readonly List<AssetMovement> _movements = new();
    private readonly List<AssetMaintenance> _maintenanceRecords = new();
    private readonly List<AssetDocument> _documents = new();

    public IReadOnlyCollection<AssetMovement> Movements => _movements.AsReadOnly();
    public IReadOnlyCollection<AssetMaintenance> MaintenanceRecords => _maintenanceRecords.AsReadOnly();
    public IReadOnlyCollection<AssetDocument> Documents => _documents.AsReadOnly();

    protected Asset() { } // For EF Core

    public Asset(
        AssetTag assetTag,
        string assetName,
        AssetCategory category,
        AssetLocation location,
        AssetFinancialInfo financialInfo,
        AssetOwnership ownershipType,
        DepreciationMethod depreciationMethod,
        int usefulLifeYears,
        Guid tenantId,
        string? description = null,
        string? model = null,
        string? manufacturer = null,
        string? serialNumber = null)
    {
        if (string.IsNullOrWhiteSpace(assetName))
            throw new ArgumentException("Asset name cannot be empty", nameof(assetName));
        if (usefulLifeYears <= 0)
            throw new ArgumentException("Useful life years must be positive", nameof(usefulLifeYears));

        Id = Guid.NewGuid();
        AssetTag = assetTag;
        AssetName = assetName.Trim();
        Description = description?.Trim();
        Category = category;
        Status = AssetStatus.Draft;
        Condition = AssetCondition.New;
        Location = location;
        FinancialInfo = financialInfo;
        OwnershipType = ownershipType;
        DepreciationMethod = depreciationMethod;
        UsefulLifeYears = usefulLifeYears;
        TenantId = tenantId;
        Model = model?.Trim();
        Manufacturer = manufacturer?.Trim();
        SerialNumber = serialNumber?.Trim();
        CreatedDate = DateTime.UtcNow;

        // Domain Event
        AddDomainEvent(new AssetCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            AssetTag,
            AssetName,
            Category,
            TenantId,
            DateTime.UtcNow));
    }

    // Asset Lifecycle Management
    public void Activate()
    {
        if (Status == AssetStatus.Available)
            return;

        var previousStatus = Status;
        Status = AssetStatus.Available;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetStatusChangedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousStatus,
            Status,
            DateTime.UtcNow));
    }

    public void AssignToUser(Guid userId, string? reason = null)
    {
        if (Status != AssetStatus.Available)
            throw new InvalidOperationException("Can only assign available assets");

        var previousUserId = AssignedToUserId;
        AssignedToUserId = userId;
        Status = AssetStatus.InUse;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        // Record movement
        var movement = new AssetMovement(
            AssetMovementType.Assignment,
            DateTime.UtcNow,
            Location,
            Location,
            previousUserId,
            userId,
            reason);
        _movements.Add(movement);

        AddDomainEvent(new AssetAssignedDomainEvent(
            Guid.NewGuid(),
            Id,
            userId,
            previousUserId,
            DateTime.UtcNow));
    }

    public void ReturnFromUser(string? reason = null)
    {
        if (!AssignedToUserId.HasValue)
            throw new InvalidOperationException("Asset is not currently assigned");

        var previousUserId = AssignedToUserId;
        AssignedToUserId = null;
        Status = AssetStatus.Available;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        // Record movement
        var movement = new AssetMovement(
            AssetMovementType.Return,
            DateTime.UtcNow,
            Location,
            Location,
            previousUserId,
            null,
            reason);
        _movements.Add(movement);

        AddDomainEvent(new AssetReturnedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousUserId!.Value,
            DateTime.UtcNow));
    }

    public void TransferToLocation(AssetLocation newLocation, string? reason = null)
    {
        var previousLocation = Location;
        Location = newLocation;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        // Record movement
        var movement = new AssetMovement(
            AssetMovementType.Transfer,
            DateTime.UtcNow,
            previousLocation,
            newLocation,
            AssignedToUserId,
            AssignedToUserId,
            reason);
        _movements.Add(movement);

        AddDomainEvent(new AssetLocationChangedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousLocation,
            newLocation,
            DateTime.UtcNow));
    }

    public void SetUnderMaintenance(string reason)
    {
        if (Status == AssetStatus.UnderMaintenance)
            return;

        var previousStatus = Status;
        Status = AssetStatus.UnderMaintenance;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetStatusChangedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousStatus,
            Status,
            DateTime.UtcNow));
    }

    public void SetOutOfOrder(string reason)
    {
        var previousStatus = Status;
        Status = AssetStatus.OutOfOrder;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetStatusChangedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousStatus,
            Status,
            DateTime.UtcNow));
    }

    public void UpdateCondition(AssetCondition newCondition, string? notes = null)
    {
        var previousCondition = Condition;
        Condition = newCondition;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetConditionChangedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousCondition,
            newCondition,
            notes,
            DateTime.UtcNow));
    }

    public void Dispose(string reason)
    {
        if (Status == AssetStatus.Disposed)
            return;

        var previousStatus = Status;
        Status = AssetStatus.Disposed;
        DisposalDate = DateTime.UtcNow;
        DisposalReason = reason;
        AssignedToUserId = null;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetDisposedDomainEvent(
            Guid.NewGuid(),
            Id,
            reason,
            DateTime.UtcNow));
    }

    // Maintenance Management
    public void ScheduleMaintenance(
        MaintenanceType type,
        DateTime scheduledDate,
        string description,
        MaintenancePriority priority = MaintenancePriority.Medium)
    {
        var maintenance = new AssetMaintenance(
            type,
            scheduledDate,
            description,
            priority);

        _maintenanceRecords.Add(maintenance);
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetMaintenanceScheduledDomainEvent(
            Guid.NewGuid(),
            Id,
            maintenance.Id,
            type,
            scheduledDate,
            priority,
            DateTime.UtcNow));
    }

    public void CompleteMaintenance(Guid maintenanceId, decimal cost, string? notes = null)
    {
        var maintenance = _maintenanceRecords.FirstOrDefault(m => m.Id == maintenanceId);
        if (maintenance == null)
            throw new InvalidOperationException($"Maintenance record {maintenanceId} not found");

        maintenance.Complete(cost, notes);
        
        // Return to available if it was under maintenance
        if (Status == AssetStatus.UnderMaintenance)
        {
            Status = AssetStatus.Available;
        }

        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetMaintenanceCompletedDomainEvent(
            Guid.NewGuid(),
            Id,
            maintenanceId,
            cost,
            DateTime.UtcNow));
    }

    // Document Management
    public void AddDocument(string documentType, string fileName, string filePath, string? description = null)
    {
        var document = new AssetDocument(documentType, fileName, filePath, description);
        _documents.Add(document);
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetDocumentAddedDomainEvent(
            Guid.NewGuid(),
            Id,
            document.Id,
            documentType,
            fileName,
            DateTime.UtcNow));
    }

    // Financial Management
    public void UpdateFinancialInfo(AssetFinancialInfo newFinancialInfo)
    {
        var previousInfo = FinancialInfo;
        FinancialInfo = newFinancialInfo;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new AssetFinancialInfoUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            previousInfo,
            newFinancialInfo,
            DateTime.UtcNow));
    }

    public decimal CalculateCurrentValue()
    {
        if (DepreciationMethod == DepreciationMethod.NoDepreciation)
            return FinancialInfo.PurchasePrice;

        var ageInYears = FinancialInfo.PurchaseDate.HasValue 
            ? (DateTime.UtcNow - FinancialInfo.PurchaseDate.Value).TotalDays / 365.25
            : 0;

        return DepreciationMethod switch
        {
            DepreciationMethod.StraightLine => CalculateStraightLineDepreciation(ageInYears),
            DepreciationMethod.DecliningBalance => CalculateDecliningBalanceDepreciation(ageInYears),
            _ => FinancialInfo.CurrentValue
        };
    }

    private decimal CalculateStraightLineDepreciation(double ageInYears)
    {
        var depreciableAmount = FinancialInfo.PurchasePrice - FinancialInfo.SalvageValue;
        var annualDepreciation = depreciableAmount / UsefulLifeYears;
        var totalDepreciation = (decimal)ageInYears * annualDepreciation;
        
        return Math.Max(
            FinancialInfo.PurchasePrice - totalDepreciation,
            FinancialInfo.SalvageValue);
    }

    private decimal CalculateDecliningBalanceDepreciation(double ageInYears)
    {
        var depreciationRate = 1.0m / UsefulLifeYears;
        var currentValue = FinancialInfo.PurchasePrice;
        
        for (int year = 0; year < (int)ageInYears; year++)
        {
            currentValue *= (1 - depreciationRate);
        }
        
        return Math.Max(currentValue, FinancialInfo.SalvageValue);
    }

    // Warranty Management
    public void SetWarranty(AssetWarranty warranty)
    {
        Warranty = warranty;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();
    }

    // Specification Management
    public void UpdateSpecifications(AssetSpecification specifications)
    {
        Specifications = specifications;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();
    }

    public void UpdateDimensions(AssetDimensions dimensions)
    {
        Dimensions = dimensions;
        LastModifiedDate = DateTime.UtcNow;
        MarkAsModified();
    }

    // Business Logic Validation
    public bool CanBeAssigned()
    {
        return Status == AssetStatus.Available && 
               !AssignedToUserId.HasValue && 
               Condition != AssetCondition.BeyondRepair;
    }

    public bool RequiresMaintenance()
    {
        var overdueMaintenance = _maintenanceRecords
            .Where(m => m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate < DateTime.UtcNow)
            .Any();

        return overdueMaintenance || 
               Status == AssetStatus.OutOfOrder || 
               Condition == AssetCondition.Poor;
    }

    public bool IsWarrantyValid()
    {
        return Warranty?.IsActive ?? false;
    }

    public TimeSpan? TimeUntilWarrantyExpiry()
    {
        return Warranty?.RemainingTime;
    }
}
