using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs;

public class CompanySettingsDto
{
    public string CompanyName { get; set; } = "";
    public string Name { get; set; } = "";
    public string TaxNumber { get; set; } = "";
    public string TaxId { get; set; } = "";
    public string Description { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public string Country { get; set; } = "";
    public string Website { get; set; } = "";
    public string Industry { get; set; } = "";
    public string FoundedYear { get; set; } = "";
    public CurrencyCode Currency { get; set; } = CurrencyCode.USD;
    public string TimeZone { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string LogoUrl { get; set; } = "";
    public string Language { get; set; } = "";
} 
