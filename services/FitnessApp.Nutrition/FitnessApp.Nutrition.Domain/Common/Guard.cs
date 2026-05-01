using FitnessApp.Nutrition.Domain.Exceptions;

namespace FitnessApp.Nutrition.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new DomainException($"{parameterName} cannot be null or empty");
        }

        public static void AgainstNegativeValue(float value, string parameterName = "Value")
        {
            if (value < 0)
                throw new DomainException($"{parameterName} cannot be negative");
        }

        public static void AgainstEmptyGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
                throw new DomainException($"{parameterName} cannot be empty");
        }
    }
}
