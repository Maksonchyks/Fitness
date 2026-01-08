using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Services;

namespace FitnessApp.Identity.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public (string Hash, string Salt) Hash(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty", nameof(plainPassword));

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);

            return (hash, salt);
        }

        public bool Verify(string plainPassword, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
        }
    }
}
