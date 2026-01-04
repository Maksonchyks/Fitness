using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailException("Email cannot be empty");

            if (!IsValidEmail(value))
                throw new InvalidEmailException("Invalid email format");

            Value = value.ToLower().Trim();
        }

        public static Email Create(string email)
        {
            return new Email(email);
        }
        private static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}
