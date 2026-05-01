using FitnessApp.Nutrition.Domain.Entities;

namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IWeightLogRepository
    {
        Task AddAsync(WeightLog log, CancellationToken ct = default);
        Task<IEnumerable<WeightLog>> GetByUserIdAsync(Guid userId, int days, CancellationToken ct = default);
    }
}
