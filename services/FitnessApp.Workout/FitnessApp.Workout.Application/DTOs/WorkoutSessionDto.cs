using System;
using System.Collections.Generic;

namespace FitnessApp.Workout.Application.DTOs
{
    public class WorkoutSessionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TrainingDayId { get; set; }
        public DateTime Date { get; set; }
        public List<ExerciseSetDto> PerformedExercises { get; set; } = new();
    }
}
