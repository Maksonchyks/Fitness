using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;

namespace FitnessApp.Workout.Domain.ValueObjects
{
    public class ProgramProfile : ValueObject
    {
        protected ProgramProfile() { }
        public Guid UserId { get; }
        public FitnessGoal Goal { get; }
        public Intensity Intensity { get; }
        public PowerMetrics? PowerMetrics { get; }

        public ProgramProfile(
            Guid userId,
            FitnessGoal goal,
            Intensity intensity,
            PowerMetrics? powerMetrics)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            UserId = userId;
            Goal = goal;
            Intensity = intensity;
            PowerMetrics = powerMetrics;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return UserId;
            yield return Goal;
            yield return Intensity;
            yield return PowerMetrics;
        }
    }
}
