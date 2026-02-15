using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Common;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Domain.Entities
{
    public sealed class ExerciseSet
    {
        public Guid Id { get; private set; }
        public Guid TrainingDayId { get; private set; }
        public TrainingDay? TrainingDay { get; private set; }
        public ExerciseType ExerciseType { get; private set; }
        public float Weight { get; private set; }
        public int Reps { get; private set; }
        public int Sets { get; private set; }
        public DateTime CreatedOn { get; private set; }
        private ExerciseSet(Guid trainingDayId, ExerciseType exerciseType, float weight, int reps, int sets)
        {
            Id = Guid.NewGuid();
            TrainingDayId = trainingDayId;
            ExerciseType = exerciseType;
            Weight = weight;
            Reps = reps;
            Sets = sets;
            CreatedOn = DateTime.UtcNow;
        }

        public static ExerciseSet Create(Guid trainingDayId, ExerciseType exerciseType, float weight, int reps, int sets)
        {
            Guard.AgainstNegativeValue(weight);
            Guard.AgainstNegativeValue(reps);
            Guard.AgainstNegativeValue(sets);

            return new ExerciseSet(trainingDayId, exerciseType, weight, reps, sets);
        }
    }
}
