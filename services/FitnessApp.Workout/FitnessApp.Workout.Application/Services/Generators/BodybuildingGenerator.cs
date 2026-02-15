using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class BodybuildingGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.Bodybuilding;
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
            var metrics = profile.PowerMetrics ?? throw new DomainException("Power metrics required for BB calculation");

            var bench = metrics.BenchPressWeight;
            var squat = metrics.SquatWeight;
            var deadlift = metrics.DeadliftWeight;

            if (profile.Intensity == Intensity.Low)
            {
                if (dayNumber == 1) // Upper Body
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.70f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 50, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ShoulderPress, 20, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 55, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SkullCrusher, 20, 12, 3));
                    return day;
                }
                if (dayNumber == 2) // Lower Body
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.65f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.RomanianDeadlift, deadlift * 0.60f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 45, 12, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 40, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.Plank, 0, 60, 3));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // Push (Chest, ShoTriceps)
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.75f, 8, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.InclineDumbbellPress, 25, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 40, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 30, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ChestFly, 15, 12, 3));
                    return day;
                }
                if (dayNumber == 2) // Legs
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.70f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegPress, 140, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegCurl, 35, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.StandingCalfRaise, 50, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HangingLegRaise, 0, 15, 3));
                    return day;
                }
                if (dayNumber == 3) // Pull (Back, RearBiceps)
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.75f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 60, 10, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 50, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 20, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BarbellCurl, 25, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HammerCurl, 15, 12, 3));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber == 1) // Chest & Triceps
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BenchPress, bench * 0.75f, 10, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.InclineDumbbellPress, 30, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ChestFly, 20, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.CloseGripBenchPress, bench * 0.55f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.TricepsPushdown, 35, 12, 3));
                    return day;
                }
                if (dayNumber == 2) // Back & Biceps
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.DeadLift, deadlift * 0.65f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BentOverRow, 60, 10, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LatPulldown, 65, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCableRow, 55, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BarbellCurl, 30, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.ConcentrationCurl, 12, 12, 3));
                    return day;
                }
                if (dayNumber == 3) // Legs & Abs
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.BackSquat, squat * 0.65f, 10, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.HackSquat, 100, 10, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LegExtension, 55, 12, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.SeatedCalfRaise, 40, 15, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.AbWheelRollout, 0, 12, 3));
                    return day;
                }
                if (dayNumber == 4) // Shoulders & Weak
                {
                    day.AddExercise(ExerciseSet.Create(ExerciseType.OverheadPress, 45, 8, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.LateralRaise, 12, 12, 4));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FacePull, 25, 15, 3));
                    day.AddExercise(ExerciseSet.Create(ExerciseType.FarmersWalk, 32, 40, 3)); // 40 секунд ходьби
                    day.AddExercise(ExerciseSet.Create(ExerciseType.WristCurl, 20, 15, 3));
                    return day;
                }
            }

            return day;
        }
    }
}
