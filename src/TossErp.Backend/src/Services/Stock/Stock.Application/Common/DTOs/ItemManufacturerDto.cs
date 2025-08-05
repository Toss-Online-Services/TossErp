namespace TossErp.Stock.Application.Common.DTOs;

public class ItemManufacturerDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string ManufacturerPartNo { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 
