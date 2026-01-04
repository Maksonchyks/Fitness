using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string value, string parameterName)
        {
            if(string.IsNullOrEmpty(value))
                throw new DomainException($"{parameterName} cannot be null or empty");
        }
        public static void AgainstOutOfRange(
        int value,
        int min,
        int max,
        string parameterName)
        {
            if (value < min || value > max)
                throw new DomainException(
                    $"{parameterName} must be between {min} and {max}. Current value: {value}");
        }
        public static void AgainstFutureDate(DateTime date, string parameterName)
        {
            if (date > DateTime.UtcNow)
                throw new DomainException($"{parameterName} cannot be in the future");
        }
        public static void AgainstPastDate(DateTime date, string parameterName)
        {
            if (date < DateTime.UtcNow.AddYears(-100))
                throw new DomainException($"{parameterName} cannot be more than 100 years in the past");
        }
    }
}
