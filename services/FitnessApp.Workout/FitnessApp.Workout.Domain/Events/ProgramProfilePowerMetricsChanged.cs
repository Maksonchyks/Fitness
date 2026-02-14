using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Interfaces;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Domain.Events
{
    public sealed record ProgramProfilePowerMetricsChanged(
        Guid UserId,
        PowerMetrics Metrics) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

}
