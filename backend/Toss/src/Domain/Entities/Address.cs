namespace Toss.Domain.Entities;

public class Address : BaseEntity
{
    public string Street { get; set; } = string.Empty;
    public string? Street2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? StateProvince { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "ZA"; // South Africa
    public Location? Coordinates { get; set; }
}

