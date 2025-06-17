using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace POS.Domain.Common.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }

    public Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Country = string.Empty;
        ZipCode = string.Empty;
    }

    public Address(string street, string city, string state, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty");
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State cannot be empty");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country cannot be empty");
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("Zip code cannot be empty");

        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Address Create(string street, string city, string state, string country, string zipCode)
    {
        return new Address(street, city, state, country, zipCode);
    }

    public void Update(string street, string city, string state, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty");
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State cannot be empty");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country cannot be empty");
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("Zip code cannot be empty");

        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }

    public bool IsInCountry(string country)
    {
        return Country.Equals(country, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInState(string state)
    {
        return State.Equals(state, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInCity(string city)
    {
        return City.Equals(city, StringComparison.OrdinalIgnoreCase);
    }

    public bool HasPostalCode(string postalCode)
    {
        return ZipCode.Equals(postalCode, StringComparison.OrdinalIgnoreCase);
    }

    public Address WithAdditionalInfo(string additionalInfo)
    {
        return new Address(Street, City, State, Country, ZipCode);
    }

    public Address WithoutAdditionalInfo()
    {
        return new Address(Street, City, State, Country, ZipCode);
    }
} 
