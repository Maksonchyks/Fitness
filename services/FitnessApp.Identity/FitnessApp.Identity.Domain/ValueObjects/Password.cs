using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Common;
using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.ValueObjects
{
    public sealed class Password : ValueObject
    {
        public string Hash { get; }
        public string Salt { get; }

        private Password(string hash, string salt)
        {
            Hash = hash ?? throw new ArgumentNullException(nameof(hash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
        }

        public static Result Validate(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return Result.Failure("Password cannot be empty");

            if (plainPassword.Length < 8)
                return Result.Failure("Password must be at least 8 characters long");

            if (!plainPassword.Any(char.IsDigit))
                return Result.Failure("Password must contain at least one digit");

            if (!plainPassword.Any(char.IsUpper))
                return Result.Failure("Password must contain at least one uppercase letter");

            if (!plainPassword.Any(char.IsLower))
                return Result.Failure("Password must contain at least one lowercase letter");

            return Result.Success();
        }

        public static Password Create(string hash, string salt)
        {
            Guard.AgainstNullOrEmpty(hash, nameof(hash));
            Guard.AgainstNullOrEmpty(salt, nameof(salt));

            return new Password(hash, salt);
        }

        public bool Matches(string plainPassword, Func<string, string, string, bool> verifyFunc)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            return verifyFunc(plainPassword, this.Hash, this.Salt);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Hash;
            yield return Salt;
        }
    }
}
