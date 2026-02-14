using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Common;

namespace FitnessApp.Workout.Domain.ValueObjects
{
    public sealed class PowerMetrics : ValueObject
    {
        public float SquatWeight { get; init; }
        public float BenchPressWeight { get; init; }
        public float DeadliftWeight { get; init; }

        public PowerMetrics(float squatWeight, float benchPressWeight, float deadliftWeight)
        {
            Guard.AgainstNegativeValue(squatWeight);
            Guard.AgainstNegativeValue(benchPressWeight);
            Guard.AgainstNegativeValue(deadliftWeight);

            SquatWeight = squatWeight;
            BenchPressWeight = benchPressWeight;
            DeadliftWeight = deadliftWeight;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SquatWeight;
            yield return BenchPressWeight;
            yield return DeadliftWeight;
        }
    }
}
