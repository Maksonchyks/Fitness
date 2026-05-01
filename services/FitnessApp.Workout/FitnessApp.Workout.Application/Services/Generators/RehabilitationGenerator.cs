using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class RehabilitationGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.Rehabilitation;

        public TrainingProgram Generate(ProgramProfile profile)
        {
            var program = TrainingProgram.CreateEmpty(profile.UserId, profile);

            int daysCount = profile.Intensity switch
            {
                Intensity.Low => 2,
                Intensity.Moderate => 3,
                Intensity.High => 3, // Навіть при High не перевищуємо 3 дні для реабілітації
                _ => 2
            };

            for (int i = 1; i <= daysCount; i++)
            {
                var day = GenerateDay(program.Id, profile, i);
                program.AddTrainingDay(day);
            }

            return program;
        }

        private TrainingDay GenerateDay(Guid programId, ProgramProfile profile, int dayNumber)
        {
            var day = new TrainingDay(programId, dayNumber);
            var metrics = profile.PowerMetrics ?? throw new DomainException("Power metrics required");

            var bench = metrics.BenchPressWeight;
            var squat = metrics.SquatWeight;
            var deadlift = metrics.DeadliftWeight;

            // Реабілітація: дуже легкі ваги, контрольовані рухи, базові вправи
            if (profile.Intensity == Intensity.Low)
            {
                day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 15, 15, 2));
                day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 10, 15, 2));
                day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 20, 12, 2));
                day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 30, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 0, 20, 2));
                return day;
            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // Upper Body Rehab
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.25f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 20, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 10, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LateralRaise, 4, 15, 2));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 45, 3));
                    return day;
                }
                if (dayNumber == 2) // Lower Body Rehab
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.25f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 20, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 15, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 15, 20, 2));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 10, 3));
                    return day;
                }
                if (dayNumber == 3) // Full Body Light
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.20f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DumbbellPress, 6, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 25, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 10, 45, 2));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber == 1) // Upper Body Progressive
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.30f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 10, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.PullUps, 0, 5, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 12, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 12, 15, 3));
                    return day;
                }
                if (dayNumber == 2) // Lower Body Progressive
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.30f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegPress, 50, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 15, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 20, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 60, 3));
                    return day;
                }
                if (dayNumber == 3) // Full Body Integration
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.25f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 20, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DumbbellPress, 8, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 12, 60, 3));
                    return day;
                }
            }

            return day;
        }
    }
}
