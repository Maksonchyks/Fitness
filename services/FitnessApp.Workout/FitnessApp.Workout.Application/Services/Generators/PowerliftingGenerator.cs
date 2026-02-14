using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class PowerliftingGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.Powerlifting;
        public TrainingProgram Generate(ProgramProfile profile)
        {
            var program = TrainingProgram.CreateEmpty(profile.UserId, profile);

            int daysCount = profile.Intensity switch
            {
                Intensity.Low => 2,
                Intensity.Moderate => 3,
                Intensity.High => 4,
                _ => 3
            };

            for (int i = 1; i <= daysCount; i++)
            {
                int dayNumber = i;
                var day = GenerateDay(program.Id, profile, dayNumber);
                program.AddTrainingDay(day);
            }

            return program;
        }

        private TrainingDay GenerateDay(Guid programId, ProgramProfile profile, int dayNumber)
        {
            var day = new TrainingDay(programId, dayNumber);
            // список 2 тренувальних днів зі списком вправ

            if (profile.PowerMetrics == null)
                throw new DomainException("power metrics is required");

            var benchWeight = profile.PowerMetrics.BenchPressWeight;
            var deadliftWeight = profile.PowerMetrics.DeadliftWeight;
            var squatWeight = profile.PowerMetrics.SquatWeight;
            if (profile.Intensity == Intensity.Low)
            {
                if(dayNumber == 1)
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, benchWeight * 0.9f, 3, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.DeadLift, deadliftWeight * 0.8f, 6, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.ChestFly, 40, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.SeatedCableRow, 40, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BarbellCurl, 15, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.TricepsPushdown, 40, 12, 4));
                    
                    return day;
                }
                if (dayNumber == 2)
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squatWeight * 0.90f, 1, 2));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.RomanianDeadlift, 60, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegExtension, 60, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.OverheadPress, 40, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LateralRaise, 10, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HangingLegRaise, 0, 15, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.StandingCalfRaise, 40, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.WristCurl, 20, 10, 4));
                    return day;
                }

            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // День Грудей
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, benchWeight * 0.85f, 5, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.InclineDumbbellPress, 25, 10, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.TricepsPushdown, 35, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.RomanianDeadlift, 70, 10, 3)); // Підсобка низ
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.StandingCalfRaise, 50, 12, 3));
                    return day;
                }
                if (dayNumber == 2) // День ніг
                {

                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squatWeight * 0.85f, 5, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegPress, 120, 10, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegExtension, 50, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.SeatedCableRow, 50, 10, 4)); 
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BarbellCurl, 20, 10, 3));
                    return day;
                }
                if (dayNumber == 3) // День Тяги
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.DeadLift, deadliftWeight * 0.80f, 6, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, benchWeight * 0.70f, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LatPulldown, 60, 10, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.FacePull, 25, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegCurl, 40, 12, 3)); 
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HangingLegRaise, 0, 12, 3));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber == 1) // Силовий Присід
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squatWeight * 0.90f, 3, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HackSquat, 80, 10, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.Plank, 0, 60, 3));
                    return day;
                }
                if (dayNumber == 2) // Силовий Жим
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, benchWeight * 0.90f, 3, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.ChestFly, 40, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LateralRaise, 12, 12, 3));
                    return day;
                }
                if (dayNumber == 3) // Силова Тяга
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.DeadLift, deadliftWeight * 0.85f, 3, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BentOverRow, 60, 8, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.GoodMorning, 40, 10, 3));
                    return day;
                }
                if (dayNumber == 4) // Легкий об'ємний жим + багато підсобки
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, benchWeight * 0.70f, 10, 4)); // 70% на об'єм
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.InclineDumbbellPress, 50, 10, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.SeatedCableRow, 60, 10, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HammerCurl, 15, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.SkullCrusher, 25, 10, 3));
                    return day;
                }
            }

            return day;
        }
    }
}
