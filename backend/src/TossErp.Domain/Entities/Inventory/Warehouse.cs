using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Inventory;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    
    // Location
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "South Africa";
    
    // Contact
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    
    // Type
    public string? WarehouseType { get; set; } // Store, Warehouse, Distribution Center
    
    // Navigation properties
    public virtual ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
}

