using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;

namespace FitnessApp.Workout.Application.DTOs
{
    public class ExerciseSetDto
    {
        public Guid Id { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public string ExerciseName => ExerciseType.ToString();
        public float Weight { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
    }
}
