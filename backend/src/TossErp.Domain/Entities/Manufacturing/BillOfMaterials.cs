using TossErp.Domain.Common;
using TossErp.Domain.Events.Manufacturing;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Bill of Materials - defines the components and quantities needed to manufacture a product
/// </summary>
public class BillOfMaterials : BaseEntity
{
    public string BomNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public int Version { get; set; } = 1;
    public BomStatus Status { get; set; } = BomStatus.Draft;
    public BomType Type { get; set; } = BomType.Production;
    
    // Quantities
    public decimal Quantity { get; set; } = 1; // Base quantity this BOM produces
    public string? UnitOfMeasure { get; set; }
    
    // Costing
    public decimal MaterialCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal OverheadCost { get; set; }
    public decimal TotalCost { get; set; }
    
    // Timing
    public decimal EstimatedProductionTime { get; set; } // in hours
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    
    // Metadata
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Navigation Properties
    public ICollection<BomItem> Items { get; set; } = new List<BomItem>();
    public ICollection<BomOperation> Operations { get; set; } = new List<BomOperation>();
    
    // Business Methods
    public void Activate()
    {
        if (Status != BomStatus.Approved)
            throw new InvalidOperationException("BOM must be approved before activation");
        
        Status = BomStatus.Active;
        AddDomainEvent(new BomActivated(Id, BomNumber));
    }
    
    public void Approve(int approvedBy, string approvedByName)
    {
        if (Status != BomStatus.Draft && Status != BomStatus.UnderReview)
            throw new InvalidOperationException($"Cannot approve BOM with status {Status}");
        
        Status = BomStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedByName = approvedByName;
        ApprovedAt = DateTime.UtcNow;
        
        AddDomainEvent(new BomApproved(Id, BomNumber, approvedBy));
    }
    
    public void CalculateCosts()
    {
        MaterialCost = Items.Sum(i => i.TotalCost);
        // Labor and overhead would be calculated based on operations
        LaborCost = Operations.Sum(o => o.LaborCost);
        OverheadCost = Operations.Sum(o => o.OverheadCost);
        TotalCost = MaterialCost + LaborCost + OverheadCost;
    }
    
    public void Obsolete()
    {
        Status = BomStatus.Obsolete;
        ValidTo = DateTime.UtcNow;
        AddDomainEvent(new BomObsoleted(Id, BomNumber));
    }
}

public enum BomStatus
{
    Draft,
    UnderReview,
    Approved,
    Active,
    Obsolete
}

public enum BomType
{
    Production,      // For manufacturing
    Engineering,     // Engineering/prototype
    Planning,        // For planning purposes only
    Phantom,         // Component exists only in BOM, not in inventory
    Reference        // Reference only, not for production
}

