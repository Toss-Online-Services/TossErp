namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Address Data Transfer Object for general use
/// </summary>
public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string? Suburb { get; set; }
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? TownshipName { get; set; }
    public string? Section { get; set; }
    public string? GPS { get; set; }
}
