using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Events;
using FitnessApp.Workout.Domain.Exceptions;

namespace FitnessApp.Workout.Domain.Entities
{
    public class WorkoutSession : AggregateRoot
    {
        public Guid TrainingDayId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }

        private readonly List<ExerciseSet> _performedExercises = new();
        public IReadOnlyCollection<ExerciseSet> PerformedExercises => _performedExercises.AsReadOnly();

        private WorkoutSession(Guid trainingDayId, Guid userId, List<ExerciseSet> actualResults) : base()
        {
            TrainingDayId = trainingDayId;
            UserId = userId;
            Date = DateTime.UtcNow;

            if (actualResults != null)
            {
                _performedExercises.AddRange(actualResults);
            }
        }

        public static WorkoutSession Create(Guid trainingDayId, Guid userId, List<ExerciseSet> results)
        {
            if (results == null || !results.Any())
                throw new DomainException("Cannot record an empty workout");

            var session = new WorkoutSession(trainingDayId, userId, results);

            session.AddDomainEvent(new WorkoutCompleted(session.Id, userId, results));

            return session;
        }
    }
}
