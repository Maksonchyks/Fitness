using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.ValueObjects;

namespace FitnessApp.Identity.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher _passwordHasher;

        public PasswordService(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public Password CreatePassword(string plainPassword)
        {
            var validationResult = Password.Validate(plainPassword);
            if (validationResult.IsFailure)
                throw new ValidationException(validationResult.Error);

            var (hash, salt) = _passwordHasher.Hash(plainPassword);
            return Password.Create(hash, salt);
        }

        public bool VerifyPassword(string plainPassword, Password password)
        {
            return _passwordHasher.Verify(plainPassword, password.Hash, password.Salt);
        }
    }
}
