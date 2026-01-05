using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Common;
using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string email)
        {
            Guard.AgainstNullOrEmpty(email, nameof(email));
            var trimmedEmail = email.ToLower().Trim();

            if (!IsValidEmail(trimmedEmail))
                throw new InvalidEmailException("Invalid email format");

            return new Email(trimmedEmail);
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
