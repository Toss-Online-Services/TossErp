using POS.Domain.Enums;

namespace POS.Domain.Models;

public class PaymentMethod
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public PaymentType Type { get; set; }
    public bool IsActive { get; set; }
    public required string Description { get; set; }
    public decimal ProcessingFee { get; set; }
    public int ProcessingTimeInMinutes { get; set; }
    public bool RequiresAuthorization { get; set; }
    public required string Icon { get; set; }
    public required string Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 
