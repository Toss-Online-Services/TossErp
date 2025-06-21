namespace TossErp.Inventory.Application.DTOs;

public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Website { get; set; }
    public string? ContactInfo { get; set; }
    public int ItemCount { get; set; }
} 
