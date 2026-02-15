using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Common;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;

namespace FitnessApp.Workout.Domain.ValueObjects
{
    public sealed class ExerciseSet : ValueObject
    {
        public ExerciseType ExerciseType { get; private set; }
        public float Weight { get; private set; }
        public int Reps { get; private set; }
        public int Sets { get; private set; }
        public DateTime CreatedOn { get; private set; }
        private ExerciseSet(ExerciseType exerciseType, float weight, int reps, int sets)
        {
            ExerciseType = exerciseType;
            Weight = weight;
            Reps = reps;
            Sets = sets;
            CreatedOn = DateTime.UtcNow;
        }

        public static ExerciseSet Create(ExerciseType exerciseType, float weight, int reps, int sets)
        {
            Guard.AgainstNegativeValue(weight);
            if (reps <= 0) throw new DomainException("Reps must be greater than 0");
            if (sets <= 0) throw new DomainException("Sets must be greater than 0");

            return new ExerciseSet(exerciseType, weight, reps, sets);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ExerciseType;
            yield return Weight;
            yield return Reps;
            yield return Sets;
        }
    }
}
