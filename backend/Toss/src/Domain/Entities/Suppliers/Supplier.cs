namespace Toss.Domain.Entities.Suppliers;

public class Supplier : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyRegNumber { get; set; }
    public string? VATNumber { get; set; }
    public string? ContactPerson { get; set; }
    
    public PhoneNumber? ContactPhone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    // Alias for handlers expecting string PhoneNumber
    public string? PhoneNumber => ContactPhone?.ToString();
    
    public int? AddressId { get; set; }
    public Address? Address { get; set; }
    
    public bool IsActive { get; set; } = true;
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; } = 30;
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<SupplierProduct> SupplierProducts { get; private set; } = new List<SupplierProduct>();
}

