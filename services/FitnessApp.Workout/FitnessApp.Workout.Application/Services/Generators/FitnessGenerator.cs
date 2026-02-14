using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class FitnessGenerator : ITrainingProgramGenerator
    {
        public FitnessGoal SupportedGoal => FitnessGoal.Fitness;
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

            if (profile.Intensity == Intensity.Low)
            {
                // У фітнесі ми робимо базу щодня, але з невеликою вагою
                day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squat * 0.50f, 15, 3));
                day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, bench * 0.50f, 12, 3));
                day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LatPulldown, 40, 15, 3));
                day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HangingLegRaise, 0, 15, 3));
                day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.Plank, 0, 45, 3));
                return day;
            }

            if (profile.Intensity == Intensity.Moderate)
            {
                if (dayNumber == 1) // Push Focus
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, bench * 0.60f, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.OverheadPress, 25, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegExtension, 40, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.TricepsPushdown, 25, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.AbWheelRollout, 0, 12, 3));
                    return day;
                }
                if (dayNumber == 2) // Pull Focus
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.DeadLift, deadlift * 0.55f, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.PullUps, 0, 8, 3)); // Власна вага
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.SeatedCableRow, 45, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.FacePull, 20, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BarbellCurl, 20, 12, 3));
                    return day;
                }
                if (dayNumber == 3) // Legs/Core Focus
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squat * 0.60f, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.RomanianDeadlift, deadlift * 0.50f, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LateralRaise, 8, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.StandingCalfRaise, 30, 20, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.Plank, 0, 60, 4));
                    return day;
                }
            }

            if (profile.Intensity == Intensity.High)
            {
                if (dayNumber % 2 == 1) // Дні 1 та 3 (Upper focus)
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BenchPress, bench * 0.65f, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BentOverRow, 40, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.ShoulderPress, 20, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.DumbbellPress, 15, 12, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.FarmersWalk, 24, 60, 3)); // 60 секунд
                    return day;
                }
                else // Дні 2 та 4 (Lower focus)
                {
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.BackSquat, squat * 0.65f, 12, 4));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegPress, 100, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.LegCurl, 30, 15, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.HangingLegRaise, 0, 20, 3));
                    day.AddExercise(ExerciseSet.Create(day.Id, ExerciseType.ReverseBarbellCurl, 15, 15, 3));
                    return day;
                }
            }

            return day;
        }
    }
}
