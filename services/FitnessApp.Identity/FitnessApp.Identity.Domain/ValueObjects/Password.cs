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

        // Приватний конструктор
        private Password(string hash, string salt)
        {
            Hash = hash ?? throw new ArgumentNullException(nameof(hash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
        }

        // Статичний метод валідації (повертає Result, не Result<Password>)
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

        // ЄДИНИЙ спосіб створити Password - з готовим хешем
        public static Password Create(string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new DomainException("Hash cannot be empty");

            if (string.IsNullOrWhiteSpace(salt))
                throw new DomainException("Salt cannot be empty");

            return new Password(hash, salt);
        }

        // Додатковий метод для зручності (якщо потрібен)
        public static Password FromHash(string hash, string salt)
        {
            return Create(hash, salt);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Hash;
            yield return Salt;
        }
    }
}
