using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;

namespace FitnessApp.Workout.Domain.Interfaces.Persistence
{
    public interface IWorkoutSessionRepository
    {
        Task AddAsync(WorkoutSession session, CancellationToken ct = default);
        Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        void Update(WorkoutSession session);
    }
}
