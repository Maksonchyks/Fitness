using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Interfaces;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Domain.Events
{
    public sealed record WorkoutCompleted(
        Guid SessionId,
        Guid UserId,
        IEnumerable<ExerciseSet> PerformedExercises) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
