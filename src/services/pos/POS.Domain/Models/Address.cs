namespace POS.Domain.Models;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }

    public string PostalCode
    {
        get => ZipCode;
        set => ZipCode = value;
    }
} 
