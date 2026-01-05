using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Common;
using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.ValueObjects
{
    public sealed class PhoneNumber : ValueObject
    {
        public string Value { get; }
        public string CountryCode { get; }

        private PhoneNumber(string value, string countryCode = "+380")
        {
            Value = value;
            CountryCode = countryCode;
        }

        public static PhoneNumber Create(string phoneNumber, string countryCode = "+380")
        {
            Guard.AgainstNullOrEmpty(phoneNumber, nameof(phoneNumber));

            var cleanNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanNumber, @"^\+?\d{10,15}$"))
                throw new DomainException("Invalid phone number format");

            return new PhoneNumber(cleanNumber, countryCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return CountryCode;
        }

        public override string ToString() => $"{CountryCode} {Value}";
    }
}
