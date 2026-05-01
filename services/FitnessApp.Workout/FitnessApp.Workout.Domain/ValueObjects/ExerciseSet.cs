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
        private ExerciseSet(ExerciseType exerciseType, float weight, int reps, int sets)
        {
            ExerciseType = exerciseType;
            Weight = weight;
            Reps = reps;
            Sets = sets;
        }

        public static ExerciseSet Create(ExerciseType exerciseType, float weight, int reps, int sets)
        {
            Guard.AgainstNegativeValue(weight);
            if (reps <= 0) throw new DomainException("Reps must be greater than 0");
            if (sets <= 0) throw new DomainException("Sets must be greater than 0");

            // Забери баг щоб в будь якій цілі, кількість підходів варіювалась від 2-5 підходів
            int adjustedSets = Math.Clamp(sets, 2, 5);

            // Чим менше підходів, тим більше повторів
            int adjustedReps = adjustedSets switch
            {
                5 => 5,
                4 => 8,
                3 => 10,
                2 => 12,
                _ => 10
            };

            // Чим менша кількість повторів, тим більша вага
            float adjustedWeight = weight;
            if (weight > 0 && reps > 0)
            {
                float weightFactor = 1.0f + ((reps - adjustedReps) * 0.03f); // 3% weight change per rep diff
                adjustedWeight = (float)(Math.Round((weight * weightFactor) / 2.5) * 2.5); // Round to nearest 2.5kg
                if (adjustedWeight < 2.5f) adjustedWeight = 2.5f;
            }

            return new ExerciseSet(exerciseType, adjustedWeight, adjustedReps, adjustedSets);
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
