namespace TossErp.Shared.DTOs;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
} 
