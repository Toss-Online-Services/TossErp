namespace TossErp.Shared.DTOs;

public class Customer : CustomerDto
{
    public string LoyaltyTier { get; set; } = "None";
    public int LoyaltyPoints { get; set; }
    public string? LoyaltyCardNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Language { get; set; } = "en";
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public bool EmailSubscribed { get; set; }
    public bool SmsSubscribed { get; set; }
    public string[]? Interests { get; set; }
    public string? Notes { get; set; }
} 
