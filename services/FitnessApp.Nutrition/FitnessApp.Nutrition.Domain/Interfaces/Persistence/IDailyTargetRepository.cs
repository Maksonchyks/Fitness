using FitnessApp.Nutrition.Domain.Entities;

namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IDailyTargetRepository
    {
        Task AddAsync(DailyTarget target, CancellationToken ct = default);
        Task<DailyTarget?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
