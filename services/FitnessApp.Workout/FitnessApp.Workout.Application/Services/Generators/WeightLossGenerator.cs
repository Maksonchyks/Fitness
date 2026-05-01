using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class WeightLossGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.WeightLoss;

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

            // Схуднення: акцент на високі повторення, помірна вага, кругове тренування
            if (profile.Intensity == Intensity.Low)
            {
                day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.40f, 20, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.40f, 15, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 30, 20, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 20, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 60, 3));
                day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 16, 60, 3));
                return day;
            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // Full Body Circuit A
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.45f, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.45f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 30, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LateralRaise, 6, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 16, 60, 3));
                    return day;
                }
                if (dayNumber == 2) // Full Body Circuit B
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.40f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 20, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.PullUps, 0, 8, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 30, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 20, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 25, 25, 3));
                    return day;
                }
                if (dayNumber == 3) // HIIT-style Circuit
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.40f, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.35f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DumbbellPress, 10, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 30, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 60, 4));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber == 1) // Upper Body Circuit
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.50f, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 30, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ShoulderPress, 15, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 15, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 20, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BarbellCurl, 15, 15, 3));
                    return day;
                }
                if (dayNumber == 2) // Lower Body Circuit
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.50f, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegPress, 80, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 25, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 30, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 25, 25, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 20, 4));
                    return day;
                }
                if (dayNumber == 3) // Full Body HIIT
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.45f, 12, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 20, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.PullUps, 0, 8, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DumbbellPress, 12, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 20, 60, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 15, 3));
                    return day;
                }
                if (dayNumber == 4) // Metabolic Conditioning
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.40f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ChestFly, 10, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LateralRaise, 6, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ReverseBarbellCurl, 10, 20, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 60, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.WristCurl, 10, 20, 3));
                    return day;
                }
            }

            return day;
        }
    }
}
