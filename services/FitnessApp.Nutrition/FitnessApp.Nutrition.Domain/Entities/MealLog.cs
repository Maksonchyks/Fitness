using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Enums;
using FitnessApp.Nutrition.Domain.Exceptions;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class MealLog : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string MealName { get; private set; } = null!;
        public MealType MealType { get; private set; }
        public NutritionValue Nutrition { get; private set; } = null!;
        public DateTime LoggedAt { get; private set; }

        protected MealLog() { }

        private MealLog(Guid userId, string mealName, MealType mealType, NutritionValue nutrition) : base()
        {
            UserId = userId;
            MealName = mealName;
            MealType = mealType;
            Nutrition = nutrition;
            LoggedAt = DateTime.UtcNow;
        }

        public static MealLog Create(Guid userId, string mealName, MealType mealType, NutritionValue nutrition)
        {
            Guard.AgainstEmptyGuid(userId, nameof(UserId));
            Guard.AgainstNullOrEmpty(mealName, nameof(MealName));
            if (nutrition == null) throw new DomainException("Nutrition values are required");

            return new MealLog(userId, mealName, mealType, nutrition);
        }
    }
}
