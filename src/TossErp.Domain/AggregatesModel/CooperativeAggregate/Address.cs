using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class Address : ValueObject
    {
        public string StreetAddress { get; private set; } = string.Empty;
        public string Township { get; private set; } = string.Empty;
        public string Province { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
        public string? Landmark { get; private set; }

        public Address(string streetAddress, string township, string province, string postalCode, string? landmark = null)
        {
            if (string.IsNullOrWhiteSpace(streetAddress))
                throw new ArgumentException("Street address is required.", nameof(streetAddress));
            
            if (string.IsNullOrWhiteSpace(township))
                throw new ArgumentException("Township is required.", nameof(township));
            
            if (string.IsNullOrWhiteSpace(province))
                throw new ArgumentException("Province is required.", nameof(province));
            
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.", nameof(postalCode));

            StreetAddress = streetAddress.Trim();
            Township = township.Trim();
            Province = province.Trim();
            PostalCode = postalCode.Trim();
            Landmark = landmark?.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StreetAddress;
            yield return Township;
            yield return Province;
            yield return PostalCode;
            yield return Landmark ?? string.Empty;
        }

        public override string ToString()
        {
            var parts = new List<string> { StreetAddress, Township, Province, PostalCode };
            if (!string.IsNullOrWhiteSpace(Landmark))
                parts.Insert(1, $"Near {Landmark}");
            
            return string.Join(", ", parts);
        }
    }
} 
