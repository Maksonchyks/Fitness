using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Exceptions;

namespace FitnessApp.Workout.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new DomainException($"{parameterName} cannot be null or empty");
        }

        public static void AgainstNegativeValue(float value)
        {
            if (value < 0)
                throw new DomainException($"{value} cannot be negative");
        }
    }
}
