using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.WMS;

/// <summary>
/// Physical bin location in warehouse for advanced WMS
/// </summary>
public class BinLocation : BaseEntity
{
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string BinCode { get; set; } = string.Empty; // e.g., "A-01-05-02"
    public string? Barcode { get; set; }
    public BinType Type { get; set; }
    
    // Physical Details
    public string? Aisle { get; set; }
    public string? Row { get; set; }
    public string? Level { get; set; }
    public string? Position { get; set; }
    
    // Capacity
    public decimal? MaxWeight { get; set; }
    public decimal? MaxVolume { get; set; }
    public decimal? CurrentWeight { get; set; }
    public decimal? CurrentVolume { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsReserved { get; set; }
    public bool RequiresTemperatureControl { get; set; }
    public decimal? TemperatureMin { get; set; }
    public decimal? TemperatureMax { get; set; }
    
    // Zone
    public string? Zone { get; set; } // Receiving, Storage, Picking, Shipping
    public bool IsPickingFace { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
}

public enum BinType
{
    Standard,
    Pallet,
    Rack,
    Shelf,
    Floor,
    Cold,
    Quarantine
}
