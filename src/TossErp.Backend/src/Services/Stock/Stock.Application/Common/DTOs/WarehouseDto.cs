namespace TossErp.Stock.Application.Common.DTOs;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsGroup { get; set; }
    public string Company { get; set; } = string.Empty;
    public string? Account { get; set; }
    public bool IsRejectedWarehouse { get; set; }
    public string? WarehouseType { get; set; }
    public string? DefaultInTransitWarehouse { get; set; }
    public bool IsDisabled { get; set; }

    // Contact Information
    public string? EmailId { get; set; }
    public string? PhoneNo { get; set; }
    public string? MobileNo { get; set; }

    // Address Information
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pin { get; set; }
    public string? Country { get; set; }

    // Tree Structure
    public int Lft { get; set; }
    public int Rgt { get; set; }
} 
