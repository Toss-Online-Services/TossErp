using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerAddress : Entity
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }
        public string? UnitNumber { get; private set; }
        public bool IsDefault { get; private set; }
        public string AddressType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? AdditionalInfo { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        private CustomerAddress() { }

        public CustomerAddress(
            string street,
            string city,
            string state,
            string country,
            string postalCode,
            string addressType,
            string? unitNumber = null,
            bool isDefault = false,
            string? additionalInfo = null,
            double? latitude = null,
            double? longitude = null)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street cannot be empty");
            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City cannot be empty");
            if (string.IsNullOrWhiteSpace(state))
                throw new DomainException("State cannot be empty");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country cannot be empty");
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new DomainException("Postal code cannot be empty");
            if (string.IsNullOrWhiteSpace(addressType))
                throw new DomainException("Address type cannot be empty");

            Id = Guid.NewGuid();
            Street = street;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            UnitNumber = unitNumber;
            IsDefault = isDefault;
            AddressType = addressType;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            AdditionalInfo = additionalInfo;
            Latitude = latitude;
            Longitude = longitude;
        }

        public void UpdateAddress(
            string street,
            string city,
            string state,
            string country,
            string postalCode,
            string? unitNumber = null,
            string? additionalInfo = null,
            double? latitude = null,
            double? longitude = null)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street cannot be empty");
            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City cannot be empty");
            if (string.IsNullOrWhiteSpace(state))
                throw new DomainException("State cannot be empty");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country cannot be empty");
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new DomainException("Postal code cannot be empty");

            Street = street;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            UnitNumber = unitNumber;
            AdditionalInfo = additionalInfo;
            Latitude = latitude;
            Longitude = longitude;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void RemoveDefault()
        {
            IsDefault = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public string GetFormattedAddress()
        {
            var address = Street;
            if (!string.IsNullOrWhiteSpace(UnitNumber))
                address += $" Unit {UnitNumber}";
            address += $", {City}, {State} {PostalCode}, {Country}";
            if (!string.IsNullOrWhiteSpace(AdditionalInfo))
                address += $" ({AdditionalInfo})";
            return address;
        }
    }
} 
