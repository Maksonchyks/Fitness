using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Interfaces;

namespace FitnessApp.Workout.Domain.Events
{
    public sealed record ProgramProfileIntensityChanged(
        Guid UserId,
        Intensity NewIntensity) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

}
