using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;

namespace FitnessApp.Workout.Application.DTOs
{
    public class TrainingProgramResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public FitnessGoal Goal { get; set; }
        public Intensity Intensity { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TrainingDayDto> TrainingDays { get; set; } = new();
    }
}
