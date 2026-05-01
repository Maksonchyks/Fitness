using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Enums;
using FitnessApp.Nutrition.Domain.Exceptions;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class DailyTarget : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public NutritionGoal Goal { get; private set; }
        public NutritionValue Target { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected DailyTarget() { }

        private DailyTarget(Guid userId, NutritionGoal goal, NutritionValue target) : base()
        {
            UserId = userId;
            Goal = goal;
            Target = target;
            CreatedAt = DateTime.UtcNow;
        }

        public static DailyTarget Create(Guid userId, NutritionGoal goal, NutritionValue target)
        {
            Guard.AgainstEmptyGuid(userId, nameof(UserId));
            if (target == null) throw new DomainException("Target nutrition values are required");

            return new DailyTarget(userId, goal, target);
        }

        public void UpdateTarget(NutritionGoal goal, NutritionValue newTarget)
        {
            if (newTarget == null) throw new DomainException("Target nutrition values are required");

            Goal = goal;
            Target = newTarget;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
