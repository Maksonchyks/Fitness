using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class EnduranceGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.Endurance;

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

            // Витривалість: дуже високі повторення, низька вага, мінімальний відпочинок
            if (profile.Intensity == Intensity.Low)
            {
                day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.30f, 25, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.30f, 20, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.PullUps, 0, 6, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 90, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 14, 90, 3));
                return day;
            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // Full Body Endurance A
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.35f, 20, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.35f, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 25, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 20, 30, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 90, 4));
                    return day;
                }
                if (dayNumber == 2) // Full Body Endurance B
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.35f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 15, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 25, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 25, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 16, 90, 3));
                    return day;
                }
                if (dayNumber == 3) // Core & Stability
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.30f, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DumbbellPress, 8, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 25, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 20, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 15, 4));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber == 1) // Push Endurance
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.40f, 20, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.InclineDumbbellPress, 10, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ShoulderPress, 12, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 15, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LateralRaise, 5, 25, 3));
                    return day;
                }
                if (dayNumber == 2) // Pull Endurance
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.40f, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.PullUps, 0, 8, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 30, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 12, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BarbellCurl, 12, 20, 3));
                    return day;
                }
                if (dayNumber == 3) // Legs Endurance
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.40f, 20, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegPress, 60, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 20, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 20, 30, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 16, 90, 3));
                    return day;
                }
                if (dayNumber == 4) // Core & Conditioning
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.35f, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 12, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 20, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 90, 4));
                    return day;
                }
            }

            return day;
        }
    }
}
