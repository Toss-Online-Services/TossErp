using TossErp.Accounts.Domain.SeedWork;

namespace TossErp.Accounts.Domain.ValueObjects;

/// <summary>
/// Address value object for South African township addresses
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string? Suburb { get; private set; }
    public string City { get; private set; }
    public string Province { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public string? TownshipName { get; private set; }
    public string? Section { get; private set; } // Township section/ward
    public string? GPS { get; private set; } // GPS coordinates for remote areas
    
    public Address(string street, string city, string province, string postalCode, string country,
        string? suburb = null, string? townshipName = null, string? section = null, string? gps = null)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        Suburb = suburb;
        City = city ?? throw new ArgumentNullException(nameof(city));
        Province = province ?? throw new ArgumentNullException(nameof(province));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        TownshipName = townshipName;
        Section = section;
        GPS = gps;
    }

    public static Address Create(string street, string city, string province, string postalCode, string country,
        string? suburb = null, string? townshipName = null, string? section = null, string? gps = null)
    {
        return new Address(street, city, province, postalCode, country, suburb, townshipName, section, gps);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Suburb ?? string.Empty;
        yield return City;
        yield return Province;
        yield return PostalCode;
        yield return Country;
        yield return TownshipName ?? string.Empty;
        yield return Section ?? string.Empty;
        yield return GPS ?? string.Empty;
    }

    public override string ToString()
    {
        var address = Street;
        if (!string.IsNullOrEmpty(Suburb))
            address += $", {Suburb}";
        if (!string.IsNullOrEmpty(TownshipName))
            address += $", {TownshipName}";
        if (!string.IsNullOrEmpty(Section))
            address += $", Section {Section}";
        address += $", {City}, {Province} {PostalCode}, {Country}";
        return address;
    }
}
