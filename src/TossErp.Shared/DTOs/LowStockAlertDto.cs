namespace TossErp.Shared.DTOs;

public class LowStockAlertDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string ItemName { get; set; } = "";
    public int CurrentStock { get; set; }
    public int MinLevel { get; set; }
    public DateTime AlertDate { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string Priority => CurrentStock == 0 ? "Critical" : "Warning";
} 
