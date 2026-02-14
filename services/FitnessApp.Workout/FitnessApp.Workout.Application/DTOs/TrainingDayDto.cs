using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Workout.Application.DTOs
{
    public class TrainingDayDto
    {
        public Guid Id { get; set; }
        public int DayNumber { get; set; }
        public List<ExerciseSetDto> Exercises { get; set; } = new();
    }
}
