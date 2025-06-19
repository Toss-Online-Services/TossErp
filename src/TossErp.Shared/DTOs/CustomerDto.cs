namespace TossErp.Shared.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Company { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;
} 
