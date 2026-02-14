using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Workout.Domain.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ITrainingProgramRepository Programs { get; }
        IWorkoutSessionRepository Sessions { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
