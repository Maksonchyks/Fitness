using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Exceptions;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class WeightLog : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public float Weight { get; private set; }
        public DateTime LoggedAt { get; private set; }

        protected WeightLog() { }

        private WeightLog(Guid userId, float weight) : base()
        {
            UserId = userId;
            Weight = weight;
            LoggedAt = DateTime.UtcNow;
        }

        public static WeightLog Create(Guid userId, float weight)
        {
            Guard.AgainstEmptyGuid(userId, nameof(UserId));
            if (weight <= 0) throw new DomainException("Weight must be greater than zero");

            return new WeightLog(userId, weight);
        }
    }
}
