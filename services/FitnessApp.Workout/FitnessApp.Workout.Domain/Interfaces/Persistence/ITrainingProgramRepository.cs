using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;

namespace FitnessApp.Workout.Domain.Interfaces.Persistence
{
    public interface ITrainingProgramRepository
    {
        Task AddAsync(TrainingProgram program, CancellationToken ct = default);
        Task<TrainingProgram?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<TrainingProgram>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        void Remove(TrainingProgram program);
    }
}
