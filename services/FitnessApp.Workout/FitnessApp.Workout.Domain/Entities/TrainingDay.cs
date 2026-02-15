using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Domain.Entities
{
    public class TrainingDay
    {
        public Guid Id { get; private set; }
        public Guid TrainingProgramId { get; private set; }
        public TrainingProgram? TrainingProgram { get; private set; }
        public int DayNumber { get; private set; }

        private readonly List<ExerciseSet> _exercises = new();
        public IReadOnlyCollection<ExerciseSet> Exercises => _exercises.AsReadOnly();
        public TrainingDay(Guid trainingProgramId, int dayNumber)
        {
            Id = Guid.NewGuid();
            TrainingProgramId = trainingProgramId;
            DayNumber = dayNumber;
        }
        public void AddExercise(ExerciseSet exerciseSet)
        {
            _exercises.Add(exerciseSet);
        }
    }
}
