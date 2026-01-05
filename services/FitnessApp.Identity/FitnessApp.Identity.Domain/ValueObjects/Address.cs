using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Common;

namespace FitnessApp.Identity.Domain.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Country { get; }
        public string City { get; }
        public string Street { get; }
        public string PostalCode { get; }
        public string? ApartmentNumber { get; }

        private Address(
            string country,
            string city,
            string street,
            string postalCode,
            string? apartmentNumber = null)
        {

            Country = country;
            City = city;
            Street = street;
            PostalCode = postalCode;
            ApartmentNumber = apartmentNumber;
        }

        public static Address Create(
            string country,
            string city,
            string street,
            string postalCode,
            string? apartmentNumber = null)
        {
            Guard.AgainstNullOrEmpty(country, nameof(country));
            Guard.AgainstNullOrEmpty(city, nameof(city));
            Guard.AgainstNullOrEmpty(street, nameof(street));
            Guard.AgainstNullOrEmpty(postalCode, nameof(postalCode));
            return new Address(country, city, street, postalCode, apartmentNumber);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return City;
            yield return Street;
            yield return PostalCode;
            yield return ApartmentNumber ?? string.Empty;
        }

        public override string ToString()
        {
            return ApartmentNumber != null
                ? $"{Street}, {ApartmentNumber}, {City}, {PostalCode}, {Country}"
                : $"{Street}, {City}, {PostalCode}, {Country}";
        }
    }
}
